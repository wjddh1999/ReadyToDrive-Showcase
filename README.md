# ReadyToDrive
### Manual Transmission Driving Simulator

클러치·기어·RPM 상태를 연결해 수동 운전의 조작 규칙을 구현한 Unity 개인 프로젝트입니다.  
**Unity · C# · WheelCollider · Windows**

[프로젝트 상세 및 시연](https://app.notion.com/p/3cba971ce6a28169aa52d709d0c70a4c)

## Overview

| 항목 | 내용 |
|---|---|
| 개발 기간 | 2024.11 – 2025.03 |
| 개발 형태 | 개인 프로젝트 |
| 핵심 경험 | 수동 변속 규칙 · 차량 상태 · 입력과 UI 연결 |
| 기반 기술 | Unity 2022.3 · C# · Rigidbody · WheelCollider |
| 공개 범위 | 직접 구현한 변속 로직의 발췌와 설명 |

## Contribution & Attribution

Unity Standard Assets 차량 제어 코드를 기반으로 사용하고, 수동 변속·클러치·RPM 규칙과 관련 입력·UI를 구현했습니다.

- **기반 코드 활용:** 차량 물리 제어와 WheelEffects 등 Standard Assets 구성 요소
- **직접 구현 영역:** 클러치 입력에 따른 기어 전환, RPM 조건별 변속 판정, 변속 후 RPM 변화, 기어 선택 UI와 차량 상태 연결
- **이 저장소에서 제외:** Standard Assets 원본, 외부 에셋, 씬, 사운드, 전체 Unity 프로젝트

차량 물리 엔진 전체를 자체 구현한 프로젝트가 아닙니다. 기어별 속도·토크 설정과 RPM 임계값을 사용한 게임플레이 모델이며, 실제 기어비·엔진 토크 곡선에 기반한 정밀 차량 시뮬레이션과 구분합니다.

## Implementation

### Clutch & Gear State

클러치를 누르면 현재 기어를 기억하고 중립으로 전환합니다. 기어 UI는 선택할 기어만 변경하며, 클러치를 놓을 때 변속 조건을 판정합니다.

| 상태 값 | 의미 |
|---|---|
| Gear | 현재 기어: -1 시동 꺼짐, 0 중립, 1–5 전진, 6 후진 |
| CacGear | 클러치를 누르기 전 기어 |
| ShiftGear | UI에서 선택한 다음 기어 |
| RPMChange | 변속에 따른 RPM 보간 진행 여부 |

### Shift Rules

- 같은 기어를 선택하면 원래 기어를 복원합니다.
- N/1단에서 1/2단으로 변경하는 경우 별도 허용 분기를 사용합니다.
- 일반적인 한 단 상향 변속은 RPM 2000 이상에서 허용하고, 미달하면 이전 기어로 복원합니다.
- 두 단 이상 상향 변속은 기어 차이에 비례한 추가 RPM을 요구하며, 미달하면 시동 꺼짐 상태로 전환합니다.
- 상향 변속 후 RPM은 감소 방향, 하향 변속 후에는 증가 방향으로 보간합니다.

## Code Samples

다음은 원본 `Assets/Scripts/CarState.cs`와 `CarCtrl.cs`에서 발췌한 코드입니다. 주변 클래스·필드·분기는 생략했으며, 독립 실행용 코드가 아닙니다. 원본 로직을 보존하고 발췌 위치와 의존성을 함께 설명합니다.

### 1. 클러치 입력과 기어 선택

`CarState.GearShift()`의 클러치 입력 분기입니다.

```csharp
if (Input.GetKeyDown(KeyCode.LeftShift) && Gear != -1)
{
    ShiftGear = Gear;
    CacGear = Gear;
    CalcLowAcc = m_LowSpeed;
    Gear = 0;
    isClutch = true;
}
```

`CarCtrl.Start()`의 중립 버튼 등록 부분입니다. 버튼은 현재 기어를 즉시 바꾸지 않고 선택값만 기록합니다.

```csharp
if (GearBtn[0] != null)
{
    GearBtn[0].onClick.AddListener(() =>
    {
        if (CarState.inst.isClutch == true && CarState.inst.Gear != -1)
            CarState.inst.ShiftGear = 0;
    });
}
```

**확인할 점:** 입력 → 선택값 기록 → 클러치 해제 시 판정으로 조작의 순서를 표현했습니다.

### 2. 상향 변속 조건

`CarState.GearShift()`의 두 단 이상 상향 변속 분기 내부입니다. 이 코드에 도달하기 전에 같은 기어·저단 예외·한 단 상향 변속 분기를 처리합니다.

```csharp
if (2000 + (500 * (ShiftGear - CacGear)) <= RPM)
{
    RPMChange = true;
    Gear = ShiftGear;
    GearSound.PlayOneShot(GearSound.clip, 0.7f);
}
else if (RPM < 2000 + (500 * (ShiftGear - CacGear)))
{
    Gear = -1;
    m_LowSpeed = 0;
    ShiftGear = 0;
    SoundCtrl(audioSource, "Vehicle_Car_Stop_Engine_Exterior", false);
}
```

**확인할 점:** 기어 차이와 RPM을 조건으로 성공·실패 상태를 구분합니다. 오디오 호출은 프로젝트 내부 의존성이며 사운드 파일은 포함하지 않습니다.

### 3. 상향 변속 후 RPM 변화

`CarState.RpmDiffuse()`에서 `RPMChange == true`이고 `CacGear < ShiftGear`인 분기입니다.

```csharp
if (RPM <= 1000.0f)
    RPMChange = false;

RPM = Mathf.Lerp(RPM, 0.0f, Time.deltaTime * 2.0f);
```

**확인할 점:** 변속 후 RPM을 즉시 대입하지 않고 보간합니다. 원본은 0을 보간 목표로 사용하고 RPM 1000 이하에서 종료 플래그를 변경합니다. 정확히 1000으로 고정하는 로직은 아닙니다.

## Retrospective

현재 코드의 특징과 이후 개선 방향을 구분합니다. 아래 개선 사항은 구현 완료를 주장하는 항목이 아닙니다.

- **상태 표현:** 원본은 정수 기어 값에 시동 상태까지 포함합니다. 이후에는 엔진 상태와 기어 enum을 분리할 수 있습니다.
- **책임 분리:** CarState에 입력·변속·계기판·연료·오디오가 함께 있습니다. 변속 판정과 표시 계층을 분리하면 규칙 테스트가 쉬워집니다.
- **설정 관리:** 기어별 속도·토크와 RPM 임계값은 코드에 직접 작성돼 있습니다. 설정 데이터로 옮기면 튜닝과 검증을 분리할 수 있습니다.
- **검증:** RPM 2000 경계, 다단 변속 실패, 클러치 해제 시점, 후진 전환을 테스트 대상으로 삼을 수 있습니다.
- **보간 종료:** RPM 목표값과 종료 조건을 일치시키고, 변속 취소·엔진 정지 시 보간 상태 초기화를 명시하는 방향으로 개선할 수 있습니다.

이번 쇼케이스 정리는 문서와 코드 발췌 작업입니다. Unity 실행·물리 테스트나 원본 코드 리팩터링은 수행하지 않았습니다.

## Repository Scope

원본 Unity 프로젝트는 Private으로 유지합니다. 이 저장소에는 검토용 문서와 직접 구현 영역의 발췌만 포함하며, 전체 빌드에 필요한 의존성과 에셋을 제공하지 않습니다.

코드와 문서는 포트폴리오 검토 목적으로 공개합니다. 별도 라이선스가 없는 자료에 대한 복제·재배포 권한을 부여하지 않습니다. 외부 구성 요소의 권리는 해당 권리자에게 있습니다.

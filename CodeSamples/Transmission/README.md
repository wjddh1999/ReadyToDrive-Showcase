# Manual Transmission Flow

이 폴더는 ReadyToDrive의 수동 변속 처리 중 클러치 입력, 기어 선택, 변속 판정, RPM 반응 흐름을 선별한 코드 샘플입니다.

```mermaid
flowchart LR
    A[클러치 입력] --> B[현재 기어 보관]
    B --> C[선택 기어 기록]
    C --> D[변속 조건 판정]
    D --> E[기어 적용]
    E --> F[RPM 변화]
```

## 파일 구성

| 파일 | 역할 |
|---|---|
| `ClutchInput.cs` | 현재 기어를 보관하고 클러치 입력 중 중립 상태로 전환 |
| `GearSelection.cs` | UI 입력으로 다음 기어를 기록하고 실제 적용 시점과 분리 |
| `ShiftValidation.cs` | 기어 차이와 RPM을 기준으로 다단 상향 변속 성공·실패 판정 |
| `RpmTransition.cs` | 변속 후 RPM을 보간해 계기판과 엔진 반응을 연속적으로 표현 |

## 처리 순서

1. 클러치 입력 시 현재 기어를 `CacGear`에 보관합니다.
2. 차량의 현재 기어를 중립으로 바꾸고 `isClutch` 상태를 활성화합니다.
3. UI는 `ShiftGear`에 다음 기어만 기록합니다.
4. 클러치를 놓을 때 현재 RPM과 기어 차이를 기준으로 변속 가능 여부를 판정합니다.
5. 성공하면 선택 기어를 적용하고, 실패하면 이전 기어 복원 또는 시동 정지 상태로 전환합니다.
6. 변속 방향에 따라 RPM을 증가 또는 감소 방향으로 보간합니다.

## 상태 구분

| 상태 | 의미 |
|---|---|
| `Gear` | 현재 적용된 기어 |
| `CacGear` | 클러치 입력 직전 기어 |
| `ShiftGear` | UI에서 선택한 다음 기어 |
| `isClutch` | 클러치 입력 상태 |
| `RPMChange` | 변속 후 RPM 보간 진행 여부 |

## 구현 범위

Unity Standard Assets의 차량 물리 제어와 `WheelEffects`를 기반 구성으로 사용했습니다. 이 폴더의 샘플은 직접 구현한 수동 변속 규칙과 입력·상태·UI 연결을 보여줍니다.

코드는 원본 `CarState.cs`와 `CarCtrl.cs`의 관련 구간을 선별한 검토용 발췌입니다. 주변 필드, UI 구성, 오디오 및 Standard Assets 의존성을 포함하지 않아 단독 실행되지 않습니다.

# Manual Transmission Flow

ReadyToDrive의 수동 변속을 클러치 입력부터 차량 구동 반영까지 연속해서 확인할 수 있는 코드 샘플입니다.

```mermaid
flowchart LR
    A[클러치 입력] --> B[기어 선택]
    B --> C[변속 판정]
    C --> D[기어 적용]
    D --> E[RPM 반응]
    E --> F[구동 토크]
```

## 파일 구성

| 파일 | 역할 | 검토 포인트 |
|---|---|---|
| [`ManualTransmissionFlow.cs`](ManualTransmissionFlow.cs) | 클러치 입력, 전체 변속 분기, 변속 후 RPM 변화 | 상태 전환과 조기 반환을 이용한 변속 규칙 |
| [`GearSelectionInput.cs`](GearSelectionInput.cs) | N·1~5·R UI 입력을 선택 기어에 연결 | 기어 선택과 실제 적용 시점 분리 |
| [`VehiclePowertrainIntegration.cs`](VehiclePowertrainIntegration.cs) | 클러치·기어 상태를 가속 입력, 토크, RPM에 반영 | 변속 상태와 차량 제어의 연결 경계 |

## 전체 흐름

1. 클러치 입력 시 현재 기어를 `CacGear`에 보관하고 차량을 중립으로 전환합니다.
2. UI 입력은 `ShiftGear`만 변경하며 현재 기어에는 즉시 반영하지 않습니다.
3. 클러치를 놓으면 후진, 동일 기어, 저단 구간, 한 단 상향, 다단 상향, 하향 변속 순서로 조건을 판정합니다.
4. 변속에 성공하면 선택 기어를 적용하고, 조건이 부족하면 이전 기어 복원 또는 시동 정지 상태로 전환합니다.
5. 상향·하향 변속에 맞춰 RPM을 보간합니다.
6. 차량 제어 계층은 클러치 상태에서 가속을 차단하고 현재 기어에 맞춰 휠 토크를 적용합니다.

## 주요 상태

| 상태 | 의미 |
|---|---|
| `Gear` | 현재 적용된 기어 |
| `CacGear` | 클러치 입력 직전 기어 |
| `ShiftGear` | UI에서 선택한 다음 기어 |
| `isClutch` | 클러치 입력 상태 |
| `RPMChange` | 변속 후 RPM 보간 진행 여부 |

## 샘플 기준

`ManualTransmissionFlow.cs`는 원본 `CarState.cs`의 상태값, `GearShift()`, `RpmDiffuse()`를 중심으로 선별했습니다.

`GearSelectionInput.cs`와 `VehiclePowertrainIntegration.cs`는 원본 `CarCtrl.cs`의 기어 버튼 등록, 클러치 가속 차단, 기어별 토크, RPM 입력 처리를 검토하기 쉬운 책임 단위로 옮긴 공개용 코드입니다. 핵심 조건과 처리 순서는 원본을 따르며, 중복 UI 등록과 이 샘플에 필요하지 않은 조향·브레이크·라이트·트랙션 로직은 제외했습니다.

Unity Standard Assets의 차량 물리 제어와 `WheelEffects`는 기반 구성으로 사용했으며 직접 구현 범위에 포함하지 않습니다. 원본 프로젝트의 씬, 오디오, UI 참조와 외부 에셋을 포함하지 않아 이 폴더만으로는 실행되지 않습니다.

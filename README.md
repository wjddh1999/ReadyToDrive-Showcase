<div align="center">

# ReadyToDrive

### Manual Transmission Driving Simulator

클러치·기어 선택·RPM 조건을 연결해 수동 운전의 판단 과정을 구현한 Unity 개인 프로젝트입니다.

![Unity 2022.3 LTS](https://img.shields.io/badge/Unity-2022.3_LTS-000000?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Gameplay_Code-512BD4?logo=csharp&logoColor=white)
![Platform Windows](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows&logoColor=white)
![Public Showcase](https://img.shields.io/badge/Status-Public_Showcase-2EA44F)

[**▶ Gameplay Video**](https://www.youtube.com/watch?v=qmOj5I5J68Y) · [**🧩 Review Code Samples**](#code-samples)

[Overview](#overview) · [Contribution Scope](#contribution-scope) · [Core Systems](#core-systems) · [Code Samples](#code-samples) · [Tech Stack](#tech-stack)

</div>

---

## Overview

| 항목 | 내용 |
|---|---|
| Genre | Manual Transmission Driving Simulator |
| Development | 2024.11 – 2025.03 |
| Team | 1명 |
| Role | Unity Client / Gameplay Programmer |
| Engine | Unity 2022.3 LTS |
| Repository | 채용 검토용 Showcase |
| Project Detail | [Notion Portfolio](https://app.notion.com/p/3cba971ce6a28169aa52d709d0c70a4c) |

### Project Focus

| Manual Transmission | Vehicle State | Player Feedback |
|---|---|---|
| 클러치 입력, 기어 선택, 변속 판정 | 현재·이전·선택 기어와 RPM 상태 | 기어 UI, 계기판, 엔진 사운드 연결 |

## Contribution Scope

Unity Standard Assets의 차량 물리 제어와 `WheelEffects`를 기반 구성으로 사용했습니다. 직접 구현한 영역은 수동 변속 규칙과 입력·상태·UI 연결입니다.

- 클러치 입력에 따른 현재 기어 보관과 중립 전환
- UI에서 선택한 기어의 적용 시점 제어
- 기어 차이와 RPM을 이용한 변속 성공·실패 판정
- 상향·하향 변속에 따른 RPM 변화
- 기어 선택 UI와 차량 상태 표시 연결

차량 물리 제어, `WheelEffects`, 외부 에셋은 직접 구현 범위에 포함하지 않습니다.

## Core Systems

### 1. Clutch & Gear Selection

클러치를 누르면 현재 기어를 별도로 보관한 뒤 차량을 중립 상태로 전환합니다. UI 입력은 다음 기어만 선택하며, 실제 기어 변경은 클러치를 놓는 시점에 판정합니다.

| 상태 | 역할 |
|---|---|
| `Gear` | 현재 기어: -1 시동 정지, 0 중립, 1–5 전진, 6 후진 |
| `CacGear` | 클러치 입력 직전 기어 |
| `ShiftGear` | UI에서 선택한 다음 기어 |
| `RPMChange` | 변속 후 RPM 보간 진행 여부 |

### 2. Shift Validation

같은 기어 선택, 저단 전환, 한 단 상향, 두 단 이상 상향을 서로 다른 규칙으로 처리합니다.

- 같은 기어를 선택하면 이전 기어를 복원합니다.
- N/1단에서 1/2단으로 전환할 때는 별도 허용 분기를 사용합니다.
- 한 단 상향 변속은 RPM 2000 이상에서 허용합니다.
- 두 단 이상 상향 변속은 기어 차이에 비례한 추가 RPM을 요구합니다.
- 조건을 충족하지 못하면 이전 기어 복원 또는 시동 정지 상태로 전환합니다.

### 3. RPM Transition

상향 변속에서는 RPM을 감소 방향으로, 하향 변속에서는 증가 방향으로 보간합니다. 기어 변경 직후 값을 즉시 바꾸지 않아 계기판과 엔진 반응이 연속적으로 보이도록 했습니다.

### 4. Vehicle Feedback

기어 선택 UI, 현재 기어, RPM 표시, 엔진 사운드를 차량 상태와 연결했습니다. 입력 결과와 변속 성공 여부를 화면과 소리로 확인할 수 있습니다.

## Code Samples

| 영역 | 코드 | 확인할 수 있는 내용 |
|---|---|---|
| Manual Transmission | [`CodeSamples/Transmission`](CodeSamples/Transmission/README.md) | 클러치 입력, 기어 선택과 적용 시점 분리, RPM 기반 변속 판정, 변속 후 RPM 반응 |

## Technical Decisions

- **Deferred gear application:** UI 입력과 실제 기어 적용 시점을 분리해 클러치 조작 순서를 표현했습니다.
- **Explicit gear state:** 현재 기어, 이전 기어, 선택 기어를 구분해 변속 판정에 필요한 상태를 보존했습니다.
- **RPM-gated shifting:** 상향 변속 성공 조건을 기어 차이와 RPM으로 계산해 조작 결과에 규칙을 부여했습니다.
- **Interpolated feedback:** 변속 직후 RPM을 즉시 대입하지 않고 보간해 시각·청각 피드백의 연속성을 유지했습니다.
- **Dependency boundary:** Standard Assets 기반 차량 제어와 직접 구현한 변속 규칙의 범위를 구분했습니다.

## Tech Stack

| 분류 | 기술 |
|---|---|
| Engine | Unity 2022.3 LTS |
| Language | C# |
| Vehicle | Rigidbody · WheelCollider · Unity Standard Assets |
| Gameplay | Clutch · Gear State · RPM-based Shift Rules |
| UI / Feedback | Unity UI · RPM / Gear Display · Audio |
| Tools | Git · GitHub · Notion |

## Repository Scope

이 저장소는 채용 검토용 Showcase입니다. 원본 Unity 프로젝트는 라이선스가 있는 외부 에셋과 Standard Assets 구성 요소를 포함하고 있어 Private으로 유지합니다.

공개 범위에는 다음 항목만 포함합니다.

- 직접 구현한 수동 변속 규칙의 핵심 코드
- 입력·차량 상태·UI 연결 구조 설명
- 프로젝트 상세 문서와 플레이 영상 연결

코드 샘플은 핵심 로직을 검토하기 위한 선별본이며, 원본 프로젝트의 전체 의존성과 에셋을 포함하지 않아 단독 실행되지 않습니다.

## Usage Notice

이 저장소의 코드와 문서는 포트폴리오 검토 목적으로 공개합니다. 별도 라이선스가 명시되지 않은 자료의 복제·재배포 권한은 부여하지 않습니다. 외부 구성 요소의 권리는 해당 권리자에게 있습니다.

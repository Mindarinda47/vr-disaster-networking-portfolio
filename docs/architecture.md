# 네트워크 구조

## 실행 역할

- **PC Host/Server:** 세션 시작, VR 플레이어 생성, 관제 화면과 미니맵 상호작용 관리
- **VR Client:** 서버 접속, XR Origin 활성화, HMD·양손 위치 갱신
- **Vivox:** SDK와 제공 샘플을 활용한 음성 서비스 연동

```mermaid
sequenceDiagram
    participant PC as PC Host / Server
    participant NM as NetworkObjectManager
    participant VR as VR Client

    PC->>PC: StartHost()
    VR->>PC: StartClient() 후 연결
    VR->>PC: LoadVRSceneServerRPC(clientId)
    PC->>PC: VR Player 생성 및 SpawnAsPlayerObject
    PC->>VR: 플레이어 소유권과 Client 화면 상태 설정
    PC->>NM: NetworkObject 등록
    NM-->>PC: 등록 완료 후 관제 카메라 연결
    VR-->>PC: HMD·양손 Transform 동기화
    PC->>PC: 미니맵 입력을 월드 좌표로 변환
    PC->>VR: 마커·화재 NetworkObject 동기화
```

## 권한 설계

| 대상 | 권한 | 선택 이유 | 관련 코드 |
| --- | --- | --- | --- |
| VR 플레이어 생성 | Server | 접속 Client ID에 맞춰 생성과 등록을 일관되게 관리 | [`NetworkConnect.cs`](../src/networking/NetworkConnect.cs) |
| HMD·양손 Transform | Owner Client | VR 입력 반응성을 유지 | [`NetworkPlayer.cs`](../src/networking/NetworkPlayer.cs), [`NetworkTransformClient.cs`](../src/networking/NetworkTransformClient.cs) |
| 미니맵 마커·화재 | Server | 모든 사용자에게 동일한 공유 상태 제공 | [`minimapClickHandler.cs`](../src/networking/minimapClickHandler.cs) |
| 층 이동 요청 | ServerRpc → ClientRpc | 요청 주체와 적용 결과를 네트워크에 반영 | [`PlayerSpawnPlace.cs`](../src/networking/PlayerSpawnPlace.cs) |

## 생성 순서 처리

플레이어가 생성되기 전에 관제 카메라나 이동 UI가 접근하면 참조가 비어 있을 수 있습니다. `NetworkObjectManager`가 생성된 오브젝트를 등록하고, 의존 기능은 등록 완료를 기다린 뒤 초기화하도록 구성했습니다.

## 관제 상호작용 흐름

```mermaid
sequenceDiagram
    participant PC as PC 관제자
    participant Map as 미니맵
    participant Server as Host / Server
    participant VR as VR 참가자

    PC->>Map: 위치 클릭
    Map->>Map: UI 좌표 정규화
    Map->>Server: 미니맵 카메라 Raycast로 월드 좌표 계산
    Server->>VR: 마커 또는 화재 NetworkObject 생성·동기화
    VR-->>PC: HMD·양손 Transform 동기화
    PC->>PC: 참가자 머리 Transform을 따라 시점 표시
    PC-->>VR: 마커·음성으로 실시간 피드백
```

### 마커와 화재의 상태 규칙

- 안내 마커는 한 개만 유지하며 새 위치를 선택하면 기존 마커를 제거합니다.
- 화재는 기본 최대 5개까지 유지하고, 한도를 넘으면 가장 오래된 화재부터 제거합니다.
- 우클릭 또는 제거 버튼으로 현재 마커·화재를 네트워크에서 `Despawn`합니다.
- 생성과 제거는 Host/Server 여부를 확인한 뒤 수행해 공유 상태를 한쪽에서 관리합니다.

이 상호작용은 단순한 맵 편집이 아니라 관제자가 참가자의 이동 목표와 위험 요소를 세션 중 변경해 실시간 피드백과 난이도 조절을 제공하기 위한 기능입니다.

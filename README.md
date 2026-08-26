# VR Disaster Simulation — Networking Portfolio

VR 재난 상황에서 여러 사용자가 함께 훈련할 수 있도록 멀티플레이 연결, 플레이어 생성과 동기화, PC 관제 연동을 구현한 3인 팀 프로젝트의 네트워크 담당 선별본입니다.

- 개발 기간: 약 3개월
- 팀 구성: 3인
- 담당: 네트워크 연결과 동기화
- 기술: Unity, C#, Unity Netcode for GameObjects, Vivox
- 원본: <https://github.com/Mindarinda47/vr-disaster-simulation>

## 담당 구현

- Host/Client 연결과 IP 입력 흐름
- 서버 권한 기반 NetworkObject 생성
- ServerRpc와 대상 ClientRpc를 이용한 요청·응답
- 클라이언트별 XR Origin 활성화
- 플레이어 생성 위치와 이동 흐름
- 클라이언트 권한 Transform 동기화 설정
- PC 관제 화면과 VR 플레이어 카메라 연결
- 미니맵 입력과 네트워크 오브젝트 연동
- Vivox 기반 실시간 음성 채팅 연동

## 선별 코드

`src/networking`에는 역할 문서와 사용자 확인을 바탕으로 네트워크 담당 후보 스크립트 11개만 포함했습니다. 전체 Unity 프로젝트와 팀원의 다른 기능 코드는 복제하지 않았습니다.

## 문서

- [네트워크 구조](docs/architecture.md)
- [기여 범위](docs/contribution.md)
- [증거와 한계](docs/verification.md)

## 주의

이 저장소는 전체 게임을 단독 실행하기 위한 배포본이 아니라 네트워크 구현을 설명하기 위한 포트폴리오 선별본입니다. Vivox를 연동했지만 별도의 음성 코덱이나 전송 프로토콜을 직접 구현한 것은 아닙니다.


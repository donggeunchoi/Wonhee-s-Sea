# 원희의 바다 (Wonhee's Sea)

바다 던전에서 물고기를 잡고, 경영 씬으로 돌아와 어획물을 가공하고 판매하는 Unity 2D 팀 프로젝트입니다. 경영 씬에 배치한 매대의 레시피에 따라 던전에 등장할 물고기 후보가 달라집니다.

## 참여자 및 담당 역할

| 이름 | 담당 역할 | 작업 내용 |
| --- | --- | --- |
| 이선정 | 전체 기획 | 게임 전체 기획을 담당했습니다. |
| 이광민 | 던전 개발 | 바다 던전의 플레이어 조작, 전투, 물고기 행동 등 던전 플레이를 담당했습니다. |
| 최다혜 | 아트 및 기획 | 게임 아트 작업과 기획에 참여했습니다. |
| 이원희 | 손님·결제 개발 및 기획 | 경영 씬의 손님 NPC와 상품 결제 기능을 담당하고 기획에 참여했습니다. |
| 최동근 | 경영·상점·서버 개발 | 경영 씬의 생산과 판매 연동, 던전과 경영 씬 간 이동 및 어획물 전달, 상점 UI와 장비 데이터, 뒤끝 로그인 및 서버 데이터 연동을 담당했습니다. |

## 플레이 흐름

1. **로그인** — 게스트 로그인 후 서버 데이터를 불러오면 경영 씬으로 이동합니다.
2. **던전 입장** — 경영 씬에 배치된 매대의 레시피를 확인해 필요한 물고기 ID를 던전 스폰 후보로 전달합니다.
3. **탐색과 어획** — 바다 던전에서 조이스틱으로 이동하고 무기를 사용해 물고기를 잡습니다. 플레이 중 산소와 인벤토리 무게를 확인할 수 있습니다.
4. **복귀와 생산** — 던전 결과를 확인하고 복귀하면 어획물이 경영 씬의 냉장고에 반영됩니다. 냉장고에서 재료를 꺼내 도마, 아궁이, 조리대에 투입해 결과물을 만들 수 있습니다.
5. **판매와 상점** — 완성품을 매대에 진열하면 손님이 가져가고, 결제가 끝나면 판매가에 따라 골드를 획득합니다. 경영 씬의 상점에서 카테고리별 아이템 목록과 상세 정보를 확인할 수 있습니다.

## 주요 구현

| 기능 | 구현 내용 |
| --- | --- |
| 생산과 진열 | 플레이어가 매대에 접근하면 재료 투입과 결과물 수거가 이루어집니다. 재료 두 개를 투입한 뒤 추가 투입이 멈추던 문제를 수정해 매대의 5개 제한 안에서 연속 투입할 수 있게 했습니다. |
| 어획물 전달 | 던전 결과 처리에서 물고기를 종류별로 집계하고 사용자 데이터에 반영합니다. 결과 화면과 복귀 버튼에서 수량이 중복 저장되던 흐름을 결과 처리 한 곳으로 정리했습니다. |
| 상점과 장비 데이터 | 상점 카테고리 전환, 스크롤 목록, 아이템 아이콘과 상세 팝업을 구현했습니다. 장비 강화 레벨 기록과 장착 정보 저장·로드까지 연결했습니다. |
| 서버 데이터 연동 | `BackendManager`가 뒤끝 인증과 차트 로드를 담당합니다. 정적 데이터는 `JsonManager`, 골드와 어획물 등 사용자 상태는 `UserManager`를 통해 참조합니다. |

## 개발 환경

| 항목 | 설정 |
| --- | --- |
| Unity Editor | 2022.3.62f2 |
| 언어 | C# |
| 주요 라이브러리 | UniRx, Cinemachine 2.10.5, Input System 1.14.2, TextMesh Pro 3.0.7, Newtonsoft Json 3.2.2 |
| 입력 설정 | 기존 Input Manager와 Input System을 함께 사용하는 `Both` 설정 |
| 서버 | 뒤끝(BACKND) SDK, 게스트 인증, 차트, 사용자 데이터 저장·로드 |
| Android 최소 API | API 24 |

## 실행 및 빌드

1. Unity Hub에서 **Unity 2022.3.62f2**를 설치하고 저장소 루트 폴더를 프로젝트로 엽니다.
2. 패키지와 에셋 임포트가 끝나면 **File → Build Settings**에서 아래 씬이 등록되어 있는지 확인합니다.

   | 빌드 순서 | 씬 | 용도 |
   | --- | --- | --- |
   | 0 | [LoginScene](Assets/02%20Scenes/LoginScene.unity) | 인증, 서버 데이터 로드 |
   | 1 | [TownScene_Stage1](Assets/02%20Scenes/TownScene_Stage1.unity) | 생산, 진열, 손님 결제, 상점, 던전 입장 |
   | 2 | [Gwang_DungeonScene](Assets/02%20Scenes/Gwang_DungeonScene.unity) | 탐색, 전투, 어획 결과 및 복귀 |

3. 에디터에서 `LoginScene`을 열고 **Play → 게스트 로그인**을 선택합니다. 로그인과 데이터 로드가 끝나면 경영 씬으로 이동합니다.
4. Android 빌드는 Unity Hub에서 해당 에디터의 **Android Build Support**, **Android SDK & NDK Tools**, **OpenJDK**를 설치한 뒤 **Build Settings → Android → Switch Platform → Build**로 진행합니다.

### 뒤끝 설정

- 다른 뒤끝 프로젝트에 연결하려면 [TheBackend 설정 에셋](Assets/TheBackend/Resources/TheBackendSettings.asset)과 `LoginScene`의 `BackendManager` Inspector를 대상 프로젝트에 맞게 설정합니다.
- `BackendManager`의 차트 설정에는 물고기, 아이템, 매대, 장비, 재료, 매대 레시피, 레시피 투입 재료, 진열대, 상점에 해당하는 차트 ID가 필요합니다.
- 에디터에서는 게스트 로그인으로 확인합니다. Google 로그인은 Android 기기 빌드에서 별도의 Google 인증 설정과 함께 확인합니다.
- 경영 씬으로 넘어가지 않으면 Unity Console에서 `[Login]`, `[Backend]` 로그를 확인합니다. 초기화 및 인증 상태, 차트 ID, `LoginManager`의 `nextSceneName`이 `TownScene_Stage1`인지 순서대로 확인합니다.

## 소스 구조

| 경로 | 주요 내용 |
| --- | --- |
| [Assets/02 Scenes](Assets/02%20Scenes) | 로그인, 경영, 던전 씬 |
| [Assets/03 Scripts/Operation](Assets/03%20Scripts/Operation) | 매대, 냉장고, 손님 NPC, 계산대, 경영 인벤토리 |
| [Assets/03 Scripts/Sea](Assets/03%20Scripts/Sea) | 던전 입력, 플레이어, 무기, 물고기, 복귀 결과 |
| [Assets/03 Scripts/Shop](Assets/03%20Scripts/Shop) | 상점 팝업과 판매 아이템 처리 |
| [Assets/03 Scripts/Manager](Assets/03%20Scripts/Manager) | 로그인, 뒤끝, 사용자 데이터, 차트 및 UI 관리 |

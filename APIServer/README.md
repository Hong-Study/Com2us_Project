# API Server

## 설명
- Game API 서버로써, ASP.NET Core 8로 제작되었습니다.
- **수정 예정**
- Redis의 SendNX 기능을 사용하여, 유저당 하나의 요청만 처리하도록 구현하였습니다.

## 서버 기능
### 유저 관련
|     **기능**     |          **완료 여부**          |
| :--------------: | :-----------------------------: |
|   로그인 체크    | <input type="checkbox" checked> |
|   유저  로그인   | <input type="checkbox" checked> |
| 게임 데이터 로드 | <input type="checkbox" checked> |
|  유저 로그 아웃  |     <input type="checkbox">     |
|    매칭 기능     | <input type="checkbox" checked> |

### 메일 기능
|     **기능**     |          **완료 여부**          |
| :--------------: | :-----------------------------: |
|    메일 전송     | <input type="checkbox" checked> |
|  메일 전체 읽기  | <input type="checkbox" checked> |
|    메일 삭제     |     <input type="checkbox">     |
| 메일 아이템 기능 |     <input type="checkbox">     |
|    메일 수령     |     <input type="checkbox">     |

### 출석 기능
|        **기능**        |          **완료 여부**          |
| :--------------------: | :-----------------------------: |
|       출석 체크        | <input type="checkbox" checked> |
| 출석 체크 후 메일 전송 | <input type="checkbox" checked> |
|     출석 정보 조회     | <input type="checkbox" checked> |

### ERD
![alt text](../resource/GameERD.png)

## 기존 방식과의 비교
- [스마일게이트 리캠프](https://github.com/sgdevcamp2023/remember/tree/main/src/backend/user-service)에서 만든 유저 서버와 비교한 내용
- **리캠프**에서는 완전 기초를 공부하기 위해 Databaes, Logger등을 직접 구현하였기 때문에 비교 X
### Controller
- **서버 캠퍼스**
  - 하나의 컨트롤러가 하나의 요청을 담당하도록 함.
  - 로그인, 로그아웃 등을 각각 하나의 컨트롤러로 구현
- **리캠프**
  - 하나의 기능에 대한 요청들을 하나의 컨트롤러가 담당하도록 함
  - 로그인, 로그아웃, 회원가입 등을 하나의 컨트롤러로 구현

- 두 가지 방식 모두 장단점을 가진다고 생각함.
  - 캠퍼스의 경우, 하나의 컨트롤러에 하나의 요청만 처리하기 때문에 컨트롤러의 코드가 길어지지 않아 유지보수에 좋다고 생각함. 
  - 다만, 요청이 많아질 수록 파일의 갯수가 많아진다는 단점이 존재.
  - 리캠프의 경우, 반대로 컨트롤러 코드의 유지보수에는 불리할 수 있으나, 기능별로 컨트롤러가 존재하기 때문에 분리되어 있다는 것이 좋다고 생각됨.
### Error
- **서버 캠퍼스**
  - 에러가 발생하는 지점에서 바로 바로 에러 처리를 진행
  
- **리캠프**
  - 에러를 하나로 처리하기 위해 에러 발생하는 지점에서 Exception을 던지도록 함.
  - 이를 Exception Middleware에서 받아서 처리하도록 구현함.
  - 단점 : Exception이 발생하는 과정에서 생기는 오버헤드.
  - 장점 : 한번에 모아서 에러를 처리할 수 있다.

### Logging
- **서버 캠퍼스**
  - Controller에서만 로그를 기록, 이 외에는 에러가 발생할 때 마다 기록
  
- **리캠프**
  - AOP 라이브러리인 `Castle.Core`를 통해 Controller - Service - Repository 을 들어갈 때 마다 로그를 기록

### 결론
- 혼자서 공부했을 때 사용했던 방법이랑 다른 부분도 많았었지만, 맞는 부분도 있었음. 이 부분을 잘 조화하면 실력 향상을 노릴 수 있을 것으로 생각됨.

## 트러블 슈팅

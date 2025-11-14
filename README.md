# 🎯 TPS Practice Range

<img width="325" height="390" alt="image" src="https://github.com/user-attachments/assets/6cbde258-ae6b-4f13-8eda-96bcec23d105" />


Unity 3D 기반으로 제작된 **TPS 사격 연습 시스템**입니다.  
무기 커스터마이징, 더미 이동/피격, 과녁 점수 시스템, 무기 인벤토리 UI 등을 포함한  
연습장 형태의 프로젝트입니다.

---

## 주요 기능

---

## 플레이어 시스템

### 조준 & 사격 (Aim & Shooting)

- 우클릭 조준(ADS) 기능  
- FOV/줌거리/스코프 배율 자동 조정  
- 좌클릭 시 총알 발사
- 샷건은 다중 펠릿 기반 발사 구조

### 무기 장착 & 교체

- 무기 슬롯(1,2번) 시스템  
- 무기 프리뷰 UI 제공  
- 드래그 앤 드롭 방식을 이용한 무기 장착 UI
- 슬롯 변경 시 weapon type에 따라 부착물 패널 자동 구성

### 부착물(Attachment) 시스템

- AttachmentSO(ScriptableObject)로 부착물 데이터 관리  
- 각 부착물은 서로 다른 스탯 변경 적용  
  - Damage, Spread, Bullet Speed, Recoil 등
- 스코프 종류에 따라 ADS 시 FOV 자동 변경 

---

## 무기 시스템

### 무기 종류

| Weapon | 설명 |
|--------|------|
| Rifle | 중간 데미지, 빠른 연사, 낮은 반동 |
| Shotgun | 다중 펠릿 발사, 큰 퍼짐, 높은 반동 |
| Sniper | 단발 고데미지, 스코프 필수 |

### 사격 처리 로직

- 총알 인스턴스 생성  
- Rigidbody로 탄환 이동 처리  
- 과녁/더미 피격 시 HitBox 배율 적용  
- 더미 혹은 과녁에 피격되면 점수 반영

### Spread & Recoil

- 무기별 퍼짐 적용  
- 반동은 카메라 pitch 상승으로 구현  

---

## 더미(Dummy) 시스템

### DummyNav (더미 이동)

- NavMeshAgent 기반 순찰 AI  
- 세 지점을 반복 경로로 이동  
- 목적지 도달 시 대기 → 다음 지점으로 이동  

### DummyHealth

- HitBox별 데미지 배율 적용

### DummySpawn

- 스폰 범위 내부에 랜덤 스폰  
- 설정된 최대 수까지 자동 생성  

---

## 과녁(Target) 시스템

### ShootingTargetScore

- 총알 맞은 위치 → 중심까지 거리 계산  

### CircleMove / ShootingTargetMove

- ShootingTargetMove: 베지어 곡선 왕복 이동  

### TargetRandomActivator

- ShootingTarget_N 들 중 랜덤 3개 활성화  
- N 라운드 반복  
- 마지막 라운드 완료 후 전체 타겟 ON

---

## ⚙️ 기술 스택

### Game Engine  
- Unity 3D (6000.2.0f1)

### Language  
- C# 기반

### Gameplay Systems  
- Cinemachine  
- NavMeshAgent  
- Rigidbody Physics  
- ScriptableObject 기반 데이터 관리  
- Coroutine(WaitForSeconds) 기반 순차 이벤트  

### UI/UX  
- Unity Canvas UI  
- TMP  
- Drag & Drop UI  
- 부착물 패널 자동 구성


### Version Control  
- Git + GitHub  


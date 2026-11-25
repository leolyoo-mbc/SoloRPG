# Implementation Plan: 임시 플레이어 캐릭터 이동

**브랜치** `codex/rpg` · **날짜** 2026-11-25 · [기능 명세](spec.md)

## Summary

Unity의 빈 테스트 공간에서 임시 캐릭터를 조종한다. 방향을 바꾸고 멈췄다가 다시 이동할 수 있으며, 카메라는 캐릭터를 따라간다.

## Technical Context

- C#과 Unity 6000.3.26f1을 사용한다. 기존 URP 17.3.0과 Input System 1.20.1을 재사용한다.
- Windows의 Unity Editor에서 플레이어 1명과 씬 1개를 검증한다. Play Mode를 종료하면 위치와 입력 상태가 초기화된다.
- 프레임 시간에 맞춰 이동량을 계산하고 입력을 놓으면 멈춘다. 검증은 [Play Mode 절차](quickstart.md)를 따른다.

## Constitution Check

헌법 파일이 미작성 템플릿이다. 설계 전후 검토에서 적용할 프로젝트 고유 규칙은 확인되지 않았다.

## Project Structure

- `Assets/_SoloRPG/Scenes/MovementPrototype.unity`에 20×20의 평평한 바닥, 중앙의 임시 캐릭터, 조명과 카메라를 둔다. 바닥에 위치 변화를 알아볼 수 있는 표식을 둔다.
- `Assets/_SoloRPG/Scripts/Prototype/`에 이동과 카메라 추적을 담당하는 MonoBehaviour를 각각 둔다. 기존 `Player/Move`를 재사용한다.
- CharacterController는 입력 벡터 크기를 최대 1로 제한하고, 초당 4 Unity 단위로 XZ 평면을 이동한다. 캐릭터 전체가 바닥 안에 머물도록 이동 범위를 제한한다.
- Play Mode 진입 시 카메라를 캐릭터 기준 (0, 5, -7)에 배치하고 캐릭터를 향해 아래로 기울인다. 이후 처음 정한 각도와 상대 위치를 유지하며, 매 프레임 캐릭터가 이동한 뒤 카메라 위치를 갱신한다.

[설계 결정](research.md) · [상태 모델](data-model.md) · [조작 계약](contracts/movement-controls.md)

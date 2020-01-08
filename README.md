ACT Timeline
============
ACT Timeline은 [ACT](http://advancedcombattracker.com/)용 플러그인입니다.  
레이드 보스 공격 패턴을 오버레이로 화면에 표시하고 특정 패턴 전에 알림을 재생할 수 있습니다.

![screenshot](https://raw.githubusercontent.com/cosmolen/act_timeline/master/doc/scrshot.gif)

## 설치
### 자동 설치
해루@모그리님의 **HAERUBOT(해루봇)** 을 이용하면 플러그인 설치와 연결을 쉽게 할 수 있습니다.  
자세한 내용은 [여기](http://www.inven.co.kr/board/ff14/4953/211)를 확인해주세요.

### 수동 설치
1. **[.NET Framework (≥ 6.0)](https://www.microsoft.com/net/download/thank-you/net472)** 를 설치합니다.
2. **[최신 릴리즈](https://github.com/cosmolen/act_timeline/releases/latest)** 를 다운로드 한 후, 원하는 폴더에 압축 해제합니다.
3. ACT를 실행합니다.

![Plugin Listing](https://raw.githubusercontent.com/cosmolen/act_timeline/master/doc/install1.png)

4. Plugin 탭 → Plugin Listing 탭에서 [Browse...]버튼을 클릭합니다.
5. 압축 해제한 폴더에서 ```BindingCoil.ACTTimeline.dll```을 선택합니다.
6. [Add/Enable Plugin]버튼을 클릭합니다.
7. Plugin 탭 → ACT 타임라인 탭을 클릭합니다.

![ACT Timeline Tab](https://raw.githubusercontent.com/cosmolen/act_timeline/master/doc/install2.png)

8. 리소스 디렉토리의 [...]버튼을 클릭하여 압축 해제한 폴더의 resources 폴더를 선택합니다.

## 제거
1. Plugin 탭 → Plugin Listing 탭에서 ```BindingCoil.ACTTimeline.dll```을 찾아 빨간색 [×]버튼을 누릅니다.
2. 플러그인 폴더를 삭제합니다.
3. ```%AppData%\Advanced Combat Tracker\Config``` 폴더에 있는 ```ACTTimeline.config``` 설정 파일을 삭제합니다.

## 사용방법
![usage](https://raw.githubusercontent.com/cosmolen/act_timeline/master/doc/usage.png)

[타임라인 작성 방법 (일본어)](doc/TimelineSyntax.md)

## 라이센스
소스 코드 및 타임라인 파일은 BSD 3-Clause 라이센스입니다.

## Special Thanks!

- RainbowMage님: TTS 대응 코드, 빌드 파일 수정

부속 wav파일은 魔王魂님의 무료 소재 재배포입니다:  
사이트: [音楽素材/魔王魂](http://maoudamashii.jokersounds.com/)  
이용 시 [소재 이용 규약](http://maoudamashii.jokersounds.com/music_rule.html)을 따라주세요.

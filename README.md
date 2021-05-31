# Altera  
该项目前身是FGOSBIAReloaded,现进行了UI重新规划并改名为Altera.  
比前代增加了从者星级、配卡图片显示.  
  
## 实现内容  
可以对FGO(JP)的所有从者的基本信息进行解析.并且可以导出为xlsx文件.  
++增加了活动列表、幕间(强化)活动新PU关卡显示、所有职介攻击侧克制关系显示功能.  
++整合了FGO(JP)资源文件的解密与重命名功能.具体操作方式请参考程序中的相关提示.(2020.11.18)  
++增加了FGO音频资源cpk文件转换wav的功能.此项功能使用了CriPakTools、DeleTore以及VGMToolBox的部分代码和类库实现.(2020.11.22)  
++增加卡池列表显示.(2020.11.27)  
++增加"加成从者(下一活动)"功能,可查看下次活动的加成从者(即游戏内显示"次回イベント対象"的选项卡).(2021.2.28)  
  
## 使用方法  
1、下载Release[(点击此处)](https://github.com/ACPudding/Altera/releases/latest "(点击此处)")中的文件.  若需要完整的dll包请下载[v3.0.0版本完整包](https://github.com/ACPudding/Altera/releases/download/v3.0.0/Altera.7z "v3.0.0版本完整包"), 再替换其中的程序本体即可.  
2、首次打开会提示"没有游戏数据",先切换至"数据更新"选项卡下载文件(如果连接过慢请事先挂一个全局梯子).  
3、在首页的文本框输入ID点击"解析"即可调出从者信息.  

## 
  
#### 一点吐槽
如遇任何兼容性问题无法保证有效修复.  
本身是作为初学C#的练手之作,写的非常烂.  
如果真的有人发现这里的话希望可以重构一个orz.  
所有对数据进行的解密部分代码使用了nishuoshenme的FGOAssetsModifyTool的代码.  
  
#### 注意事项  
1、如果不知道具体ID可以先点击"导出ID列表"按钮导出ID名单.  
2、宝具和技能的详细数据由于没有完整的规则所以通过了嵌套switch实现(该功能可以通过首页"高级功能"关闭,显示原始文件中的数值), 会有解析错误的情况发生.  
3、Buff的翻译以及特性的翻译有的时候日服刚刚更新完成的时候会出现新的未翻译或者未知的Buff, 会等Mooncell或者茹西翻译完之后添加到Github的相应读取文件中(无需更新程序本体).  
4、如果需要导出从者的基础信息,勾选首页的"高级设置"选项卡下的"询问是否导出xlsx文件"复选框即可.  
5、检查更新使用的是api.github.com,如果出现连接失败请连接全局梯子.  
6、不保证长期更新.  
7、程序中UI使用了"思源黑体", 宝具特效字体使用了"FOT-Skip"、"FOT-Matisse", 若有缺失, 可能显示不正常(字体请自行补齐, 不提供下载方式).  
8、可能之后还会添加其他功能?  

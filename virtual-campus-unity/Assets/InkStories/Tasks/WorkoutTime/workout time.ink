//mission name: workout time
//任务名：革命的本钱
#after: greetings of canteen

#name: 革命的本钱
#description: 吃完饭，或许是时候该运动一下了，快去找室内体育馆的工作人员了解一下学校里的体育设施吧！


=== func_start ===
#attach: PEA_staff
*你好，这里就是体育馆吗
是的，你是新来的同学吗，看来还不太了解我们学校的体育设施。
-
+n
那么，需要我给你介绍一下吗？
    **好的
        ->introduction


=== endings ===
+n
-
那么，这些就是目前我们学校所有的体育设施了，如果你有其他问题，欢迎给体育部发邮件，或到室内体育馆前台咨询！
*好的，再见
#endstory
->DONE

=== introduction ===
*室内体育馆设施 ->introduction.indoors
*室外体育场地 ->introduction.outdoors
*其他体育设施 ->introduction.others
*->endings

= indoors
健身房和游泳馆这些设施，相信你从名字就能了解用处，不再细讲。
-
+n 
跆拳道室：供体育课教学，学生社团或教职员团体武术训练、HIIT等社团训练使用。
-
+n 
多功能馆：内设有篮球场、排球场、羽毛球场，每天设置不同类型和数量的场地；供体育课教学，日常锻炼，训练，大小型活动使用。
-
+n 
体适能测试中心：进行体适能测试的场地。
-
+n 
舞蹈室：供体育课教学，学生社团或教职员团体舞蹈、形体训练等使用。
-
->introduction

= outdoors
我校总共有5片篮球场，其中下园2片、上园3片。
-
+n 
亦有5片网球场，其中中园3片、上园2片。
-
+n 
同时中园还有一片五人制足球场。
-
->introduction

= others
我校其他的体育设施包括：上园P栋服务中心健身房、逸夫健身房、SR3教职员健身房、乒乓球室。
-
->introduction

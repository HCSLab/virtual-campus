// mission name: blackswan
#require: greetings of library

#name: 黑天鹅事件
#description: 学校里飞进来了一只黑天鹅？这可不是什么大概率事件。目击者们各执一词，真相到底是什么样的呢？


=== func_start ===
#attach: lib_receptionist
*怎么了，看起来有些心事？
听说学校里飞进来一只黑天鹅，刚刚还在图书馆后面的水池里呢。
-
+n
我还准备等下班之后去看一眼，结果好像已经飞到别的地方了。
-
*哦，在哪里？
不知道啊，不过这种事情，学校应该会去找物业处帮忙的。
-
+n
你可以去问问他们。物业处就在<color=red>TD旁边的楼下</color>。
-
+n
毕竟黑天鹅还是挺难得一见的，也得稳妥处理，应该会在找到之后放生到附近的公园里吧。
-
*听起来有些有趣……
->DONE

=== func_PM_part ===
#require: func_start
#attach: PM_staff
*有听过黑天鹅的消息吗？
黑天鹅吗，好像听过，刚刚物业处有人去找了，但是现在也没有找到它在哪里。
-
+n 
不过，刚刚有个学生事务处的老师来这里了。
-
+n
是来找自己丢掉的饭卡的，不过她来的时候念念叨叨着什么“都怪黑天鹅”之类的话。
-
+n 
她找了半天也没找到，好像就回学生事务处了。
-
+n
结果她刚走，她的饭卡就被人送过来了。
-
*看起来她可能知道什么
是啊，所以你或许可以去<color=red>学活</color>那里找<color=red>学生事务处</color>问问看？
-
+n
顺便，你也可以把她的饭卡带给她，谢谢啦。
-
*……怎么感觉又变成跑腿了
->DONE

=== func_OSA_part ===
#require: func_PM_part
#attach: OSA_receptionist
*老师，这是你的饭卡嘛？
啊，是的是的！
-
+n
谢谢你！是你捡到的吗？
-
*啊这……不……
肯定是！看你长得这么好人，真是谢谢你了！
-
*<color=\#808080>（大概也许应该是夸我吧……)</color>
看你的样子，有什么需要帮忙的事？尽管说！
-
*额……那老师你有见过黑天鹅吗
黑天鹅？别提了，我刚刚匆匆忙忙吃完饭就是想去看看那只传闻中的黑天鹅的。
-
+n
结果我提前吃完饭，去找了半天也没见到，饭卡还丢了……
-
+n
说到底，我确实也没有见到黑天鹅……也没有什么线索可以给你
-
+n
不过，我听说，逸夫的宿管阿姨好像也去找了。
-
+n
或许你可以去<color=red>逸夫宿舍楼</color>那里找她问问……
-
*嗯嗯，谢谢
->DONE

=== func_Shaw_part ===
#require: func_OSA_part
#attach: Shaw_dorm_admin
*阿姨好，你有找到黑天鹅吗
黑色的天鹅？
-
+n
没有啊，咱们学校什么时候进来过黑天鹅吗？
-
+n
别说黑天鹅了，天鹅飞进我们学校的都够奇怪的了。
-
+n
要是有的话，肯定早就抓起来放生了。
-
*可是，您不是去找了……
没有的事，黑天鹅这种东西，就是唬人的，别想了。
-
+n
你是又听了别人随便说的话吧，有谁见过吗？
-
*没有……
那不就得咯。
-
*但是他们都说有人见到了
有人？有人又是谁呢……
-
*<color=\#808080>（好像确实没有人真正见过啊……）</color>
而且，你有见过照片嘛？一只黑天鹅飞进来，难不成没有一张照片吗……
-
*确实也没有照片……
唉，一看就是以讹传讹，阿姨我见多啦。
-
*<color=\#808080>（难道真是以讹传讹……？）</color>
好啦好啦，阿姨我也不怪你问这么奇奇怪怪的事，不过我还有别的事要忙。
-
+n
你要是没有别的事，我就得接着去忙了。拜拜。
-
*<color=\#808080>（注意到脚边有一根黑色的禽类羽毛）</color>
#additem: 黑色羽毛
#endstory
->DONE

//mission name: take delivery
#require: greetings of library
#require: missing computer

VAR SW_door = false
VAR SE_door = false
VAR N_door = false

=== func_start ===
#override
#collidetrigger: student_ZBW
好饿……
*要一起去食堂吃饭吗？
去食堂？不好意思，刚刚已经订了迈挡捞的外卖了。
-
*……
诶，刚好你来了。外卖小哥刚送到，打电话叫我去领，但是我现在在忙，实在腾不出手了，你能帮我领一下吗？
-
*我要吃饭欸
唉，可惜了，我本来还多订了一份迈响鸡块……
-
*……
唉，可惜了，我本来还多订了一份迈响鸡块……一份迈拉鸡翅……
-
*……
唉，可惜了，我本来还多订了一份迈响鸡块……一份迈拉鸡翅……还有一份薯条……
-
*在哪？
就知道你靠谱！
-
+n
额……好像是哪个大门来着……<color=blue>东南门</color>……？
-
+n
额……好像是哪个大门来着……<color=blue>东南门</color>……<color=blue>西南门</color>……？
-
+n
额……好像是哪个大门来着……<color=blue>东南门</color>……<color=blue>西南门</color>……还是<color=blue>北门</color>……？
-
+n
要不……麻烦你都去看看？
-
*嗯？真把我当跑腿的了？
哦对了对了，我记得还多订了一份中杯快乐水来着……？
-
*啥时候要？
诶，不急不急，谢谢啦。辛苦你啦，嘿嘿。
-
*……别忘了刚刚说好的跑腿费
->DONE

=== func_north_door ===
#after: func_start
#collidetrigger: north_gate_collider
~N_door = true
{SE_door == true && SW_door == true:
似乎哪里都没有，你决定回去找张博文问问看。
-else:
似乎这里并没有任何人，去别的地方看看吧。
}
+n
->DONE

=== func_southeast_door ===
#after: func_start
#collidetrigger: southeast_gate_collider
~SE_door = true
{N_door == true && SW_door == true:
似乎哪里都没有，你决定回去找张博文问问看。
-else:
似乎这里并没有任何人，去别的地方看看吧。
}
+n
->DONE

=== func_southwest_door ===
#after: func_start
#collidetrigger: southwest_gate_collider
~SW_door = true
{N_door == true && SE_door == true:
似乎哪里都没有，你决定回去找张博文问问看。
-else:
似乎这里并没有任何人，去别的地方看看吧。
}
+n
->DONE

=== func_ending === 
#after: func_north_door
#after: func_southeast_door
#after: func_southwest_door
#override
#collidetrigger: student_ZBW
啊，你来了，抱歉，我刚刚……
*三个门怎么都没有人
抱歉，我刚刚记错了，应该是小广场来着。
-
+n
貌似快递小哥都是默认送到小广场的。
-
*……那我就这么白跑了一趟？
额……确实有点不好意思……
-
+n
可是我多订的都已经给你了啊……怎么办……
-
*算了算了，你多订的那些给我就行
嗯嗯，当然，给你。
-
*话说，你为什么多订了这么多啊
唔……因为本来就准备叫你过来吃的嘛，不然我为什么订这么多。
-
+n
毕竟你也是我入学以来交的第一个朋友嘛……
-
+n
毕竟你也是我入学以来交的第一个朋友嘛……也帮了我不少忙……
-
*还算你有良心
那当然，不过我也帮了你不少啊，还帮你介绍食堂来着……下次你请我。
-
*唔，下次一定
#endstory
->END
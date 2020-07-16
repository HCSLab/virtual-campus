//mission name: take delivery
// #require: greetings of library

VAR SW_door = false
VAR SE_door = false
VAR N_door = false

=== func_start ===
#attach: student_ZBW
*要一起去食堂吃饭吗？
去食堂？我从来不去食堂吃的好吗。
-
*……
诶，刚好你在这里，我刚刚订了迈当捞的外卖。外卖小哥刚送到，打电话叫我去领，但是我现在在忙，实在腾不出手了，你能帮我领一下吗？
-
*我要吃饭欸
唉，可惜了，我本来还多订了一份迈响鸡块和迈拉鸡翅的，正愁着不知道怎么办呢……
-
*在哪？
额……好像是哪个大门来着……我也忘了……你要不……都去看看吧……？
-
*嗯？真把我当跑腿的了？
哦对了对了，我记得还多订了一份大杯可乐来着……？
-
*啥时候要？
不急不急，谢谢啦。辛苦你啦，嘿嘿。
-
*……别忘了鸡块鸡翅和可乐……
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
#attach: student_ZBW
*三个门怎么都没有人
抱歉，我刚刚记错了，应该是小广场来着。
-
+n
貌似快递小哥都是默认送到小广场的。
-
*……那我就这么白跑了一趟……？
额……确实有点不好意思……
-
+n
可是我多订的都已经给你了啊……怎么办……
-
*唉，算了算了 原有的跑腿费给我就行
唔，给你。
-
+n
那就算到下一次啦。下一次订的时候再给你订点弥补你咯。
-
*这还差不多
#endstory
->END
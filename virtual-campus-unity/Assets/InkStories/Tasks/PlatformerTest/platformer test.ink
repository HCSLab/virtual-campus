===func_headmaster_ask_you===
#attach:headmaster
*   开始跑酷！
-   冲冲冲！
*   GKD！GKD！
#enable:PlatformerLevel
->DONE

===func_finish===
#collidetrigger:PlatformerLevel/Destination
#after:func_headmaster_ask_you
谢谢你救了我！但是公主不在这里。
*   库巴等等俺！
#endstory
->DONE
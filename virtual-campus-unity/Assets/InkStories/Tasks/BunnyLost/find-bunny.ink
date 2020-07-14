===func_part1===
#attach:headmaster
*   听说兔子丢了？
    是的。我已经很多天没有见到它了。
    **  那太糟糕了
        我最近公务繁忙，没时间去找它。你能帮我找找它吗？
        *** 好的，没问题
            #enable:bunny
            ->DONE
        
===func_part2===
#collidetrigger:bunny
你发现了兔子，快去向校长汇报吧！
*   好的
    #disable:bunny
    ->DONE
    
===func_part3===
#attach:headmaster
#after:func_part2
*   我找到兔子啦！
    哇，太好了！你在哪找到的？
    **  志仁楼西边的树丛里
        太感谢你了，这枚兔子徽章送你！
        *** 谢谢校长
            #endstory
            ->DONE
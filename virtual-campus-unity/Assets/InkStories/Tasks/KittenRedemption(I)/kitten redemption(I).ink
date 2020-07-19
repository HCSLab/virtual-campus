//mission name: kitten redemption
#after: greetings of library

=== func_begin ===
#override
#collidetrigger: cat_whitey
*<color=\#808080>（啊，好可爱的猫）</color>
喵喵喵……喵喵喵……
-
*<color=\#808080>（撸猫）</color>
<color=\#808080>（猫咪挣脱掉了）</color>
喵……喵！喵！
-
*<color=\#808080>（撸猫）</color>
<color=\#808080>（猫咪挣脱掉了）</color>
喵……喵！喵！
-
*啊，小猫猫听话……
<color=\#808080>（猫咪仍然看着你，不断地叫着）</color>
喵……喵！喵！
-
*这只猫叫的好像有规律啊……一长两短的
<color=\#808080>（猫咪仍然看着你，不断地叫着）</color>
喵……喵！喵！
-
+n
<color=\#808080>（猫咪仍然看着你，不断地叫着）</color>
喵……喵！喵！喵……喵！喵！
-
+n
<color=\#808080>（猫咪仍然看着你，不断地叫着）</color>
喵……喵！喵！喵……喵！喵！喵……喵！喵！
-
+n
<color=\#808080>（猫咪仍然看着你，不断地叫着）</color>
喵……喵！喵！喵……喵！喵！喵……喵！喵！喵……喵！喵！
-
*真的诶……难道是摩斯电码？
<color=\#808080>（猫咪沉默了，似乎是对你感到无语）</color>
-
*难道这只猫会摩斯电码？
<color=\#808080>（猫咪仍然看着你，似乎叫的更急了）</color>
喵喵喵！
-
+n
<color=\#808080>（猫咪仍然看着你，似乎叫的更急了）</color>
喵喵喵！
-
*难道是要我帮它嘛？
<color=\#808080>（猫咪点了点头？！）</color>
喵！
-
*可是我听不懂，怎么办呢
<color=\#808080>（猫咪抬起爪子，指了指）</color>
喵喵喵\~
-
*顺着指的方向望去
<color=\#808080>（似乎指向的是……逸夫书院？）</color>
-
*不是会摩斯电码啊，只是能听懂人话而已
<color=\#808080>（猫咪仍然看着你，似乎叫的更急了）</color>
喵喵喵！
-
*那似乎正常很多了……不对，更不正常了啊！！！
<color=\#808080>（猫咪仍然看着你，似乎叫的更急了）</color>
喵喵喵！
-
*算了，先去逸夫那里问问看吧
<color=\#808080>（猫咪仍然看着你，似乎终于满意了）</color>
喵！
-
*不过似乎那里比较靠谱的就只有<color=blue>宿管阿姨</color>
<color=\#808080>（猫咪仍然看着你，似乎终于满意了）</color>
喵！
-
*那我去看看吧
<color=\#808080>（猫咪似乎期待地看着你）</color>
喵！
-
+n
->DONE

=== func_whitey ===
#require: func_begin
#override
#collidetrigger: cat_whitey
<color=\#808080>（猫咪似乎期待地看着你）</color>
喵！
-
+我得去找<color=blue>逸夫的宿管阿姨</color>问问这只猫的事
#notfinished
->DONE

=== func_go_to_Shaw_dorm_admin ===
#after: func_begin
#attach: Shaw_dorm_admin
*阿姨，你见过食堂门前的那只小白猫嘛
哦？看样子你已经见过小白了，很可爱是吧？
-
看来，你就是被选中的幸运儿了！我想……小白拦住你，大概有话对你讲。
-
+n
每年都有幸运的孩子被猫咪选中，有机会跟它们亲密接触。看样子你被pick了。
-
+n
你可算是好运哦，小白可是学校最知名的猫咪之一，你别看它最近灰头土脸的，洗干净的时候那也是个帅小伙……
-
+n
我是看着它长大的，在它还是小孩子的时候我就抱过过它，当然了，它现在也是个可爱的孩子。
-
+n
它才一岁啊，说来，想不到那个小毛团子，竟然也要当爸爸了。真是时光荏苒啊……
-
+n
你看我，又跑题了。你既然来这里专门问我小白的事了，那可能你也发现了吧，它能听得懂你说话……
-
+n
喵星人一般不会让人发现的哦，你能发现，肯定是小白有自己的原因不得不让你知道。
-
+n
不管怎么样，你也是被小白pick的人了，以后也需要和小白正常沟通了哦。
-
+n
那么，这台高科技喵星人机器“猫语翻译器”就可以给你了。这台机器可高科技了，听说人文学院都有参与制作哦。
-
+n
这装备在外面，你都找不到的。只有校内才提供哦，而且也极其稀少，似乎现在只有你手上的这一台了。
-
+n 
拿好吧，只要有了它，就可以和小白正常对话了。
-
*太棒了
TODO: 获取物品：猫语翻译器
->DONE

=== func_return_to_cat_whitey ===
#after: func_go_to_Shaw_dorm_admin
#override
#collidetrigger: cat_whitey
<color=\#808080>（猫咪试探地看着你）</color>
………………
*你……好？
啊，人类，你拿到翻译器了吗？
-
*天啊……真的能听懂了……
啊，似乎你没有见过能听懂人话的猫吧。咱家也很少见过能听懂猫话的人，可以理解。
-
*所以，你刚刚让我拿翻译器是为了……？
当然是为了让你帮我了，我现在可是有一件要紧的烦心事。烦躁的很呢。
-
+n
当然是为了让你帮我了，我现在可是有一件要紧的烦心事。烦躁的很呢。此刻烦躁的心情就像……
-
+n
当然是为了让你帮我了，我现在可是有一件要紧的烦心事。烦躁的很呢。此刻烦躁的心情就像……就像……
-
*就像……？
当然是为了让你帮我了，我现在可是有一件要紧的烦心事。烦躁的很呢。此刻烦躁的心情就像……就像……就像用十除以三得出的结果一样，无穷无尽。
-
*原来猫也会数学
人哪，为了消磨时间，硬是鼓唇摇舌，笑那些并不可笑、乐那些并不可乐的事，此外便一无所长。
-
+n
咱家是猫。名字嘛……还没有。不过它们都叫我小白。
-
*你叫小白啊……
咳咳，人类，既然你能听懂我说的话，这说明咱们有缘。
-
+n
那我单方面宣布你就是我的知己了，帮我一个忙吧！！！
-
*知己的要求好低
美人兮，见之不忘，一日不见兮，思之如狂。
-
*什么意思？
额……简单来说就是……
-
+n
我老婆不见了。
-
+n
我已经整整25小时47分钟58秒没有见过她了。
-
+n
我找遍了整个学校，都没办法找到她。
-
+n
她叫小可爱，是只狸花猫，下巴到胸口长着白毛。
-
+n
听路人说，最后一次见到她是在TB楼下，你可以去问问<color=blue>人文学院的院长</color>。
-
*好吧，我去问问
->DONE

=== func_whitey2 ===
#require: func_return_to_cat_whitey
#override
#collidetrigger: cat_whitey
人类，有问到关于小可爱的消息吗？
-
+好的，我会去的
#notfinished
->DONE

=== func_go_to_HSS_dean ===
#require: func_return_to_cat_whitey
#attach: HSS_dean
*院长好，你最近有见到小可爱吗
小可爱？难道她不见了？嘶，这可是个大问题。
-
+n
我记得我昨天在TB楼下见到它，挺着个肚子，我想它好像应该是怀孕了。
-
*怀……孕？？
人文学院院长：是的，我想可能要麻烦你去确认一下这件事。
-
+n
毕竟小白这个毛孩子自己还是个傻小子呢。
-
+n
我们大家也该准备好迎接新生命的到来了。
-
+n
快去问问<color=blue>宿管阿姨</color>该怎么办吧。
-
*好的
->DONE

=== func_return_to_dorm_admin ===
#require: func_go_to_HSS_dean
#attach: Shaw_dorm_admin
*阿姨，怎么办，小可爱失踪了！
小可爱失踪了？
-
*而且貌似还怀孕了……
还怀孕了？
-
+n
哦天呐……
-
+n
不过……这两件事放到一起倒解释通了，我似乎明白了小可爱为什么失踪了。
-
+n
猫咪在怀孕的时候，有的母猫食量会增加，而且会嗜睡。
-
+n
我想小可爱可能是对伙食不太满意，所以在额外觅食。
-
+n
是时候额外给它加餐了，我早该想到的。
-
+n
这里有一份<color=blue>猫粮</color>，或许可以帮助你找到它。
-
+n
对了，有好心的师生为两只猫咪准备了猫粮，放在我这里。
-
+n
本来委托我每日去喂猫，但是我这边还有点事情不能离岗。
-
+n 
如果你有空的话，可以每日来我这里领10份猫粮喂它们嘛。
-
+n 
两只猫咪和学长姐、教授们的关系都很好，知道学校不少的传闻。
-
+n 
说不定在喂食的时候，它们会开心的告诉你校园故事。
-
TODO:获取物品：猫粮
*不过，阿姨你知道小可爱在哪里嘛
小可爱平日总是跟小白一起出现。
-
+n
所以我猜，它应该还是在食堂附近。
#enableNPC: cat_cutey
-
*什么？难道我又白跑了？
怎么，不相信阿姨嘛，你大可以去看看嘛，我和这两只猫可熟了，肯定不会错。
-
*行，我这就去看看
->DONE

=== func_ending ===
#require: func_return_to_dorm_admin
#override
#collidetrigger: cat_whitey
*……
……
-
*所以搞了半天，小可爱不就在你旁边嘛……
……是这样的。
-
*那你找了25个小时都去哪了啊……
咳咳，总之呢，非常感谢你帮我找到小可爱，
-
+n
不过没想到她不见的原因竟然是怀孕饿了找吃的。
-
*……不愧是你
不管如何，能找到小可爱，多亏了你。
-
+n
如果你以后无聊的话，都欢迎来找我聊天。
-
*我和一只猫聊天干嘛……
嗯？天真的人类，你有了猫语翻译器，就可以和猫咪真真正正地聊天了，这可是多少人求之不得的机会！
-
*不好意思，没兴趣
啧……竟然会有人不喜欢猫咪……
-
+n
不过就算你不喜欢猫咪，也不代表和我聊天就没有意义哦。
-
*怎么说
跟我聊天，你可以获得一些<color=blue>别的地方打听不到的消息</color>。
-
*你一只猫能知道什么啊……
咳咳，我常年混迹校园各个角落，最擅长的就是偷听你们人类讲话，可是知道不少奇奇怪怪的事情的！
-
*似乎有道理
可不要小瞧这些情报，之后可能会在<color=blue>意想不到</color>的地方帮助你哦！
-
*就因为我今天帮了你嘛
是的，看在你今天帮我找到小可爱的份上。
-
+n
当然，这些情报也不会一下全部透露给你哦，还需要你自己争取的。
-
*争取？用什么啊
<color=blue>猫粮</color>啊。
-
+n
因为小可爱怀孕后可能胃口还会变大，可能还要你时常拿点<color=blue>猫粮</color>喂给她。
-
+n
我也是要做父亲的猫了，也得为她分担点辛苦了。我通过情报赚猫粮，也算是赚钱养家咯。
-
+n
你喂的猫粮越多，我就会在聊天中给你透露越高级的情报哦。
-
+n
因此，多带一点<color=blue>猫粮</color>来吧。
-
*听起来像是打工一样
交易，是交易。双赢哦。
-
+n
总之，就拜托你啦。
-
*好的
#endstory
->END


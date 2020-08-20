// mission name: academic career
#require: greetings of SSE
#require: greetings of SME
#require: greetings of HSS
#require: greetings of LHS
#require: greetings of SDS

#name: 学术生涯
#description: 你已经找到了所有学院的院长！现在你可以返回<color=blue>教务处老师</color>那里了，似乎她还有一些事情要和你说。

VAR school = ""
VAR index = 0
VAR egg = ""
VAR acflag = 0

=== attach_school_dean ===
{ school:
    - "SME": #attach: SME_dean
    - "SSE": #attach: SSE_dean
    - "LHS": #attach: LHS_dean
    - "HSS": #attach: HSS_dean
    - "SDS": #attach: SDS_dean
}
->->

=== function chi_school(eng_school) ===
{ eng_school:
    - "SME": ~return "经管学院"
    - "SSE": ~return "理工学院"
    - "LHS": ~return "生命健康学院"
    - "HSS": ~return "人文社科学院"
    - "SDS": ~return "数据科学院"
}

// Press "ALT" and left click to SKIP to the corresponding part
=== turn_to ===
{acflag:
    -0: ->General_Questions
    -1:
    {school:
        -"SME":->SME_Easy_Questions
        -"SSE":->SSE_Easy_Questions
        -"LHS":->LHS_Easy_Questions
        -"HSS":->HSS_Easy_Questions
        -"SDS":->SSE_Easy_Questions
    }
    -2:
    {school:
        -"SME":->SME_Hard_Questions
        -"SSE":->SSE_Hard_Questions
        -"LHS":->LHS_Hard_Questions
        -"HSS":->HSS_Hard_Questions
        -"SDS":->SSE_Hard_Questions
    }

}

=== init ===
~index = 0
~egg = ""
->->

/*************************************/
//              Main Body            //
/*************************************/
    
=== func_start_career ===
#attach: registry_receptionist
*已经把院长们全都找回来了
啊，那真是太感谢你了。
-
+n
诶对了，你是哪个学院的呢？
-
*我……我好像还没有学院来着？
怎么可能呢？一般的新生在入学之前都会选定自己的学院啊？让我看看你的资料。
-
+n
（查询资料中）
-
+n
诶，奇怪了，好像你的资料被谁修改过，现在确实是没有学院的状态。
-
+n
坏了，这可怎么办。
-
+n
这样吧，你刚刚也和四个院长见过面了，不如你现在决定一下自己想要进入哪个学院？或许你在刚刚和他们见面的时候，就听过他们对于每个学院内部各个专业的介绍了。
-
+n
如果没有的话，还是可以<color=red>各个教学楼下面</color>找各个院长详细了解一下各个学院的信息，再多去了解一些吧，结合自身情况，做好决定之后，来找我就可以了。
-
*好的
#upd_description:在和教务处老师对话之后，你意识到你还没有进入任何学院！去和<color=red>各个院长</color>再多多了解一下各个学院吧，做好决定之后，就可以返回<color=red>教务处老师</color>这里来选择学院啦！
->DONE

=== func_return_to_registry ===
#require: func_start_career
#attach: registry_receptionist
+我做好决定了
你真的确定好了吗？不需要再考虑考虑了吗？
    ++算了，我再想想
        #notfinished
        ->DONE
    ++我决定好了
    是吗，那你准备选择哪个学院呢？
-
+n
你决定选择：
    ++经管学院
    ~school = "SME"
    ++人文学院
    ~school = "HSS"
    ++生命健康学院
    ~school = "LHS"
    ++理工学院
    ~school = "SSE"
    ++数据科学院
    ~school = "SDS"
    -
是<color=red>{chi_school(school)}</color>啊，好的，那么我会为你尽快办理好入院手续的。    
-
+n
现在，去找<color=red>{chi_school(school)}的院长</color>聊聊吧，或许还会有一些别的事情要告诉你呢。
-
*知道了
#upd_info
#upd_description:你已经决定好选择的学院了！教务处老师建议你去找<color=red>对应学院的院长</color>院长聊聊，似乎在进入学院之前，院长还有一些考验。
->DONE

=== func_return_to_dean ===
#require: func_return_to_registry
-> attach_school_dean ->

*院长好，我是来申请加入学院的
啊，我刚刚接到教务处老师的通知了。
-
*那我可以入学了吗
可惜，还不行哦。
-
+n
因为你这个算是特殊情况，逾期进入学院，是特殊情况。因此我为你特殊安排了一场入院考试。
-
*入院考试？
对的，准确地说，更像是<color=blue>问答</color>。
-
+n
整个问答一共分为<color=blue>三个阶段</color>，每个阶段会有若干道题。全部为<color=blue>单选</color>。难度上会有递进。
-
+n
因为是刚刚才接到消息，因此这套题是题库里临时随机组合出来的，有的时候，<color=blue>题目会有重复</color>，你可能会在不同阶段做到同一道题，有时，你甚至会在同一阶段做到同一道题。
-
+n
因为情况特殊，学院的老师也在忙着接待新生，因此我们便发动了很多学院内的学生、以及学校里的工作人员给你丰富题库，可是费了不少功夫。
-
+n
其中有图书馆的前台人员。
-
+n
教务处的老师。
-
+n
你的学长学姐。
-
+n
<color=\#808080>甚至还有食堂前的那两只小猫……</color>
-
*小猫？
不要在意这些……总之，你只需要知道，每个学校的成员都有可能参与了这套题的编写。因此这套题既不硬核，也不轻松，或许应该说是一套杂七杂八什么都有的题。
-
+n
有的会考你对于学校某个设施或是某项规定的了解；有的会考你一些学院的常识；有的甚至会考你一些你不知道的学院内部轶事；当然，也会有几道单纯的学术问题……
-
+n
你或许会觉得很难，但是其实除了学术类问题需要完全靠你自己回答以外，<color=blue>其他的问题，你都可以在校园里找到答案</color>。
-
+n
当然，这就需要你认真地去搜集信息，打听情报。毕竟，逾期进入学院可不是那么容易的。
-
+n
而对于学术问题，有一点你必须清楚：这些题里会有一些，或许有很多，以你目前的能力也解决不了的问题。作为院长，我当然认为，你是应该在认真学习之后再回来回答这些难题的。但是呢，条条大路通罗马，解决问题的方式或许不止一种。
-
+n
对于你来说，最好的消息或许莫过于，我们允许你<color=blue>多次作答</color>。这样，你会有充足的时间去准备，不要心急，不要气馁，失败了就再接再厉。
-
+n
那么，当你准备好了的话，就回到我这里来答题吧。
-
+n
#upd_description:和院长交谈过之后，你才意识到逾期进入学院并没有那么简单。在答题之前，或许你会想再去收集收集学校里的信息。
->DONE

// 暂时弃用
// === funchint ===
// #require: func_return_to_dean
// ->attach_school_dean->
// +关于问答
//     {school:
//         -"SME":->hint.SME
//         -"SSE":->hint.SSE
//         -"LHS":->hint.LHS
//         -"HSS":->hint.HSS
//         -"SDS":->hint.SDS
//     }

=== func_career ===
#require: func_return_to_dean
#notfinished
{acflag:
    -0: ->career_1
    -1: ->career_2
    -2: ->career_3
}

=== career_1 ===
-> attach_school_dean ->
+我准备好了，开始第一轮的答题吧！
    ->init->
    ->General_Questions

=== career_2 ===
-> attach_school_dean ->
+我准备好了，开始第二轮的答题吧！
    ->init->
    ->turn_to

=== career_3 ===
-> attach_school_dean ->
~index = 0
+我准备好最后一轮的答题了！
真的嘛，这可是最终的答题了哦，你确定自己准备好了吗？
    ++是的
    那么，开始吧！
        ->init->
        ->turn_to
    ++那我再准备准备吧
        ->DONE

=== success ===
~acflag = acflag+1
{acflag:
    -1:
    恭喜你！完成了第一部分的题目。当你准备好之后，就可以来进行第二阶段的答题了！
        +n
        #upd_info
        #upd_description:你完成了第一阶段的问答，干得很好！去准备准备之后，再返回这里进行第二阶段的答题吧！
        ->DONE
    -2:
    恭喜你！完成了第二部分的题目。当你准备好之后，就可以来进行最后的答题了！
        +n
        #upd_info
        #upd_description:你完成了第二阶段的问答，干得很好！去准备准备之后，再返回这里进行第三阶段的答题吧！
        ->DONE
    -3:
    恭喜你！完成了最后的题目！
        +n
        #upd_info
        ->ending
}

=== failure ===
{egg != "": ->checkEgg}
啊呀，你这道题答错了呢，再回去多加努力吧。
+好吧……
->END

=== ending ===
+n 
那么，你已经完成了所有的问题，恭喜你成为了{chi_school(school)}的一员！
-
+n 
怎么样，这套题不容易吧。
-
+n
经历这么多磨难，终于答完题目，感觉如何，有成就感吗？
-
*感觉挺疲倦的……
辛苦了辛苦了。
-
*没事，都是为了能够顺利上学嘛……
是的，恭喜你终于进入了{chi_school(school)}。
-
+n
不过我还有一些其他的事情要告诉你。
-
+n
其实，你之前所做的那些题，并不是像我之前告诉你的那样，由学校里杂七杂八的人一起凑出来的。
-
+n
这套题其实是开学后不久，一个戴着奇怪面具的人找到我，交给我的。
-
+n
这个奇怪的人似乎知道会有一个没有进入任何学院的新生，也就是你。并且料定了这个特殊的新生会决定来{chi_school(school)}，因此把这套题交给了我。
-
+n
那个人告诉了我我之前给你说的那些话，让我这么告诉你这套题的来源，并且一定要让你做完这三套题，才能告诉事实。很奇怪的要求吧……
-
*那院长你怎么也真的照做了啊……
嘶……因为我当时拿到之后，也大概看了看这几套题，虽然不能说出的很好，但是对于新生来说，也算是各方面知识都尽量普及了，做做总没坏处。
-
+n
而且其实对于你来说，谁出的题、哪来的题也没有什么区别嘛……就算那个神秘人不给我这套题，你还是需要通过一定考验才能逾期入学的。
-
*好吧……
哦对了，还有一个东西要给你。
-
+n
<color=\#808080>（院长在兜里摸索了一阵子）</color>
-
+n
就是这个徽章，也是那个神秘人给我的，说这个叫做<color=\#800080><b>学术徽章</b></color>。说在你顺利答完所有题目之后，就可以把这个徽章给你了。
-
+n
虽然从学院院长的角度来看，这些题的内容可能和我理解的学术有一点点区别……
-
+n
不过嘛，做学术更重要的不是知识或者内容，而是<color=blue>探索、收集、分析以及解决的过程</color>。
-
+n
相信这些，你已经在之前寻找答案的过程中体会到了。
-
+n
因此这枚<color=\#800080><b>学术徽章</b></color>，我认为是应该给你的。请收好它吧。
-
*……谢谢院长
{ school:
    - "SSE": #additem: SSE学术徽章
    - "SME": #additem: SME学术徽章
    - "HSS": #additem: HSS学术徽章
    - "LHS": #additem: LHS学术徽章
    - "SDS": #additem: SDS学术徽章
}
#addskin: 学霸
#upd_description:你完成了所有的问答，成功进入了学院，并获得了一枚学术徽章作为奖励！根据院长的描述，你迄今为止所答的题目都来自一个戴着面具的神秘人……
#endstory
->END

/*************************************/
//            General Part           //
/*************************************/

=== General_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。
            ++n
            {index}. ->pickEasy
    -2: 不要轻敌哦，刚开始题目简单是很正常的，接下来就会慢慢变难咯。
            ++n
            {index}. ->pickEasy
    -3: 那么下面一道题……是有关我们学院的……
            ++n
            {index}. <>->pickSchool
    -4: 那么，下面一道……
            ++n
            {index}. <>->pickSchool
    -5: 看来你似乎觉得这些题都太简单了吗？那好啊，来点难题吧！
            ++n
            {index}. ->pickHard
    -6: 接下来就是最后一道题咯……
            ++n
            {index}. ->pickHard
    -7:->success
}

= pickEasy
{~->easy1|->easy2|->easy3|->easy4|->easy5|->easy6|->easy7}

= pickSchool
{school == "SME":{~->SME1|->SME2}}
{school == "SSE":{~->SSE1|->SSE2}}
{school == "LHS":{~->LHS1|->LHS2}}
{school == "HSS":{~->HSS1|->HSS2}}
{school == "SDS":{~->SDS1|->SDS2}}
->General_Questions

= pickHard
{~->hard1|->hard2|->hard3|->hard4|->hard5}

/* General Questions: Easy Part */
= easy1
从TC一楼的电梯上到三楼之后的楼层，实际上属于：
    + A. TD
        ~egg = "easy1.1"
        ->failure
    + B. TC
        ~egg = "easy1.1"
        ->failure
    + C. TB
        ->turn_to
    + D. TA
        ~egg = "easy1.1"
        ->failure
    
= easy2
我校的官方英文简称是：
    + CUHK(SZ)
        ->turn_to
    + CUHK
        ->failure
    + LGU
        ~egg = "easy2.1"
        ->failure
    + CUSZ(HK)
        ~egg = "easy2.2"
        ->failure
    
= easy3
我校一共有____个本科生学院：
    + 999
        ~egg = "easy3.1"
        ->failure
    + 233
        ~egg = "easy3.1"
        ->failure
    + 5
        ->turn_to
    + 0
        ~egg = "easy3.2"
        ->failure
        
= easy4
我校下园的书院有几个？
    + 天下大同，大家都在下园
        ~egg = "easy4.1"
        ->failure
    + 全部木大，大家都不在下园
        ~egg = "easy4.2"
        ->failure
    + 独步天下，只有逸夫独享下园
        ->turn_to
    + 呆滞，我不知道
        ~egg = "easy4.3"
        ->failure
        
= easy5
图书馆本科生可以进入的楼层是？
    +二至四层
        ->failure
    +一至六层
        ->failure
    +二至六层
        ->failure
    +一至五层
        ->turn_to
        
= easy6
本科生生至少需要修够___学分才能提前毕业：
    +120
        ->turn_to
    +144 
        ->failure
    +180
        ->failure
    +1200
        ~egg = "easy6.1"
        ->failure
        
= easy7
本科生在校期间需要修够至少修够___门英语课，___门通识课：
    + 4 10
        ->failure
    + 0 0
        ~egg = "easy7.1"
        ->failure
    + 4 4
        ->failure
    + 4 6
        ->turn_to
        
/* General Questions: Hard Part */
= hard1
如果你是一名本科生，想要订图书馆的房间，下面哪一个不能订呢？
    +一楼的自习室
        ->failure
    +二楼的多人静音舱
        ->turn_to
    +四楼的自习室
        ->failure
    
= hard2
如果你在丢了饭卡之后，想去失物招领处找回，可以去____。
    +TD楼下的物业处
        ->turn_to
    +TA701的失物招领处
        ~egg = "hard2.1"
        ->failure
    +校长办公室
        ~egg = "hard2.3"
        ->failure
    +TD130的无人失物招领处
        ~egg = "hard2.2"
        ->failure
    
= hard3
我校一共有___个网球场，其中___个是室外的，___个是室内的。
    +5 3 2
        ->failure
    +4 4 0
        ->failure
    +5 5 0
        ->turn_to
    +4 2 2 
        ->failure
    
= hard4
加退课一般在开学后的多长时间内？
    +两周
        ->turn_to
    +一周
        ->failure
    +一个月
        ->failure
    +一学期
        ~egg = "hard4.1"
        ->failure
        
= hard5
我校下园目前一共有几个食堂？
    +4
        ->turn_to
    +5
        ~egg = "hard5.2"
        ->failure
    +6
        ~egg = "hard5.3"
        ->failure
    +0 
        ~egg = "hard5.1"
        ->failure

/* General Questions: School Part */
// SME //
= SME1
我们学院的英文简称是？
    + SME
        ->turn_to
    + SSE
        ->failure
    + HSS
        ->failure
    + LHS
        ->failure
    + SDS
        ->failure
        
= SME2
我们学院成立于哪一年？
    + 2014
        ->turn_to
    + 2018
        ->failure
    + 2020
        ->failure
        
// SSE //
= SSE1
我们学院的英文简称是？
    + SME
        ->failure
    + SSE
        ->turn_to
    + HSS
        ->failure
    + LHS
        ->failure
    + SDS
        ->failure
        
= SSE2
下列专业或方向里，哪一个不属于我们学院？
    +新能源科学与工程
        ->failure
    +计算机科学与技术
        ->turn_to
    +金融数学
        ->failure
    +电子信息工程
        ->failure

// LHS //
= LHS1
我们学院的英文简称是？
    + SME
        ->failure
    + SSE
        ->failure
    + HSS
        ->failure
    + LHS
        ->turn_to
    + SDS
        ->failure
        
= LHS2
我们学院成立于哪一年？
    + 2014
        ->failure
    + 2018
        ->turn_to
    + 2020
        ->failure
        
        
// HSS //
= HSS1
我们学院的英文简称是？
    + SME
        ->failure
    + SSE
        ->failure
    + HSS
        ->turn_to
    + LHS
        ->failure
    + SDS
        ->failure
        
= HSS2
下列专业或方向里，哪一个不属于我们学院？
    +翻译
        ->failure
    +英语
        ->failure
    +法语
        ->turn_to
    +心理学
        ->failure
        
// SDS //
= SDS1
我们学院的英文简称是？
    + SME
        ->failure
    + SSE
        ->failure
    + HSS
        ->failure
    + LHS
        ->failure
    + SDS
        ->turn_to

= SDS2
我们学院成立于哪一年？
    + 2014
        ->failure
    + 2018
        ->failure
    + 2020
        ->turn_to

/*************************************/
//              SME Part             //
/*************************************/

=== SME_Easy_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次可基本都是真真正正的学术问题了哦。
            ++n
            {index}. ->pick
    -2: 接下来可会慢慢变难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题作为放松：
            ++n
            {index}. ->General_Questions.pickEasy
    -4: 那么，下面一道，依然是和校园有关的：
            ++n
            {index}. ->General_Questions.pickHard
    -5: 看来你似乎觉得这些题都太简单了吗？那好啊，最后一道了，来点难题吧！
            ++n
            {index}. ->pick
            
    -6:     ->success
}

= pick
{~->easy1|->easy2|->easy3|->easy4|->easy5}

= easy1
一个变量遵循正态分布，均值为10，标准差为10。每一回抽取100个样本，抽取100回。请问这100回的平均值样本方差是多少？
    +A.0
        ->turn_to
    +B.1
        ->failure
    +C.0.5
        ->failure

= easy2
假设有一个假设检测，H0：均值参数为60。在使用计算器计算后我们得到一个双边检验的P值为0.0892。已知，Ha：均值参数小于60，z值为-1.7，alpha为0.05。是否可以拒绝原假设？
    +拒绝原假设H0
        ->turn_to
    +不拒绝原假设H0
        ->failure

= easy3
一支股票假定一直持续给股东每年2块的分红，当前年利率为2.5%，当前股票价格为100元。请问该股票是否值得购入？
    +是    
        ->failure
    +否
        ->turn_to
	

= easy4
假设在一个封闭独立的国家内，有一天突然出现一次生产上的技术革命，请问市场的均衡价格会有何变化？
    +提高    
        ->failure
    +降低
        ->turn_to
    +不变
        ->failure

= easy5
假设市场上有ABC三种商品，其中A、B互为替代品，B、C分别为normal good和inferior good，当A的价格下降时，B和C的消费有什么变化？
    +B消费下降，C不变    
        ->turn_to
    +B消费提高，C消费下降    
        ->failure
    +B消费下降，C消费提高
        ->failure

=== SME_Hard_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次的问题可是会变难了哦。
            ++n
            {index}. ->pick
    -2: 接下来的题可是会更加难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题，作为送分题……
            ++n
            {index}. ->General_Questions.pickSchool
    -4: 那么，下面一道，回归一下吧……
            ++n
            {index}. ->SME_Easy_Questions.pick
    -5: 最后的最后！来点难题吧！
            ++n
            {index}. ->pick
    -6: +n
        全部正确，辛苦你了，这些题对你来说可都不容易吧。
            ->success
}

= pick
{~->hard1|->hard2|->hard3|->hard4}

= hard1 
假设有一个假设检测，H0：均值参数为50。在使用计算器计算后我们得到一个双边检验的P值为0.0124。已知，Ha：均值参数大于50，z值为-2.5，alpha为0.01。是否可以拒绝原假设？
    + A. 拒绝原假设H0
        ->failure
    + B.不拒绝原假设H0
        ->turn_to
    //（答案：不拒绝原假设H0-B）

= hard2 
假定有一家制造公司销售生产的渣渣，每年该公司生产5万吨渣渣，每吨卖4元，每吨的生产成本为2块5。这项项目的项目周期为3年，要求回报率为20%。该项目的固定成本是1万2每年，同时该公司投资了9万元到生产设备上。这些生产设备将在3年的项目周期结束后完全折旧。且该项目初期需要2万元的运营成本。税率为34%。请问项目周期的最后一年的现金流为多少？
    + A. 78,870
        ->failure
    + B. 77,780
        ->failure
    + C. 71,780
        ->turn_to
    //（答案：71,780-C）

= hard3
假设市场上有两家公司AB，它们共同面对一个线性需求曲线P(y)=a-by，边际成本恒定为C，先手为A，后手为B，请问这两个公司的Stackelberg equilibrium output是多少？
    + A. XA=(a-c)/b; XB=(a-c)/2b   
        ->failure
    + B. XA=(a-c)/2b; XB=(a-c)/4b   
        ->turn_to
    + C. XA=(a-c)/b; XB=(a-c)/3b
        ->failure
    //（答案：B）

= hard4
某位客户的效应方程是U(x,y,z)=x+(z^1/2)*f(y)
X是客户购买完别的生产物资剩下的钱，z是商品1，y是一种架子，当y>0时，f(y)=8; y=0时，f(y)=0，请问这客户会买多少个商品1？
    + A.12        
        ->failure
    + B.16        
        ->turn_to
    + C. 18
        ->failure
    //（答案：16-B）

/*************************************/
//              SSE Part             //
/*************************************/

=== SSE_Easy_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次可基本都是真真正正的学术问题了哦。
            ++n
            {index}. ->pick
    -2: 接下来可会慢慢变难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题作为放松：
            ++n
            {index}. ->General_Questions.pickEasy
    -4: 那么，下面一道，依然是和校园有关的：
            ++n
            {index}. ->General_Questions.pickHard
    -5: 看来你似乎觉得这些题都太简单了吗？那好啊，最后一道了，来点难题吧！
            ++n
            {index}. ->pick
            
    -6:     ->success
}

=pick
{~->easy1|->easy2|->easy3|->easy4}

= easy1	
在Python语言中，“ab”+“c”*2 结果是:
+abc2 
    ->failure
+abcabc 
    ->failure
+abcc
    ->turn_to
+ababcc 
    ->failure

=easy2
某烷烃发生氯代反应后，只生成三种沸点不同的一氯代产物，此烷烃是:
+ A.(CH3)2CHCH2CH2CH3
    ->failure
+ B.(CH3CH2)2CHCH3
    ->failure
+ C.(CH3)2CHCH(CH3)2 
    ->failure
+ D.(CH3)3CCH2CH3
    ->turn_to

=easy3	
若地球卫星绕地球做匀速圆周运动，其实际绕行速率:
+ A.一定等于7.9km/s 
    ->failure
+ B.一定小于7.9km/s 
    ->turn_to
+ C.一定大于7.9km/s 
    ->failure
+ D.介于7.9-11.2km/s之间 
    ->failure

=easy4 
关于地球同步通讯卫星的说法，正确的是: 
+ A.若质量加倍，则轨道半径也加倍 
    ->failure
+ B.在赤道上空⻜行 
    ->failure
+ C.以第一宇宙速度运行 
    ->failure
+ D.⻆速度等于地球自转⻆速度
    ->turn_to

=== SSE_Hard_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次的问题可是会变难了哦。
            ++n
            {index}. ->pick
    -2: 接下来的题可是会更加难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题，作为送分题……
            ++n
            {index}. ->General_Questions.pickSchool
    -4: 那么，下面一道，回归一下难度吧……
            ++n
            {index}. ->SSE_Easy_Questions.pick
    -5: 最后的最后！来点难题吧！
            ++n
            {index}. ->pick
    -6: 全部正确，辛苦你了，这些题对你来说可都不容易吧。
            ->success
}

=pick
{~->hard1|->hard2|->hard3|->hard4}

=hard1 
在使用Python语言时，导入模块的方式错误的是：
+ A. import mo
    ->failure
+ B. from mo import *
    ->failure
+ C. import mo as m
    ->failure
+ D. import m from mo
    ->turn_to
    
=hard2	
分子式为C4H9Cl的同分异构体(不含对映异构)有：
+ A. 2种 
    ->failure
+ B. 3种 
    ->failure
+ C. 4种 
    ->failure
+ D. 5种 
    ->turn_to
    
=hard3 
下列哪个语句在Python中是非法的：
+ A. x = y = z = 1 
    ->failure
+ B. x = (y = z + 1)
    ->turn_to
+ C. x, y = y, x 
    ->failure
+ D. x += y
    ->failure

=hard4
下列物质一定与丁烯互为同系物的是: 
+ A.C2H4 
    ->turn_to
+ B.C3H6 
    ->failure
+ C.C4H8 
    ->failure
+ D.C4H10 
    ->failure


/*************************************/
//              LHS Part             //
/*************************************/

=== LHS_Easy_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次可基本都是真真正正的学术问题了哦。
            ++n
            {index}. ->pick
    -2: 接下来可会慢慢变难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题作为放松：
            ++n
            {index}. ->General_Questions.pickEasy
    -4: 那么，下面一道，依然是和校园有关的：
            ++n
            {index}. ->General_Questions.pickHard
    -5: 看来你似乎觉得这些题都太简单了吗？那好啊，最后一道了，来点难题吧！
            ++n
            {index}. ->pick
            
    -6:     ->success
}

=pick
{~->easy1|->easy3|->easy4}

=easy1
LHS作为小氪金学院，购置了许多新的实验室设施和装备。你猜猜下面哪个最贵？
+5000只小白鼠
    ->failure
+一台冷冻电镜
    ->turn_to
+3台三代测序仪
    ->failure
+10台高分辨率质谱仪 
    ->failure
    
//=easy2	
//LHS（生命健康学院）有三个合作的诺奖研究院，别搜索，猜一下哪个诺奖获得者拿的是诺贝尔//生理学或医学奖？
//+科比尔卡创新药物开发研究院的Prof. Brian K. KOBILKA
//    ->failure
//+瓦谢尔计算生物研究院的Prof. Arieh Warshel
//    ->failure
//+切哈诺沃精准和再生医学研究院的Prof. Aaron Ciechanover
//    ->failure
//+虽然我们是生命健康学院，但是这奖雨我无瓜
//    ->turn_to
    
=easy3
从前有两个人，一个是正常人，一个是肾功能只剩下30%的人，他们吃相同的适量的食物。猜猜谁的尿液多？
+肯定正常人啦
    ->failure
+肾功能受损的
    ->failure
+都一样
    ->turn_to
+看情况，abc都对
    ->failure
    
=easy4
    一位工人被一条铁棍以高速从下颚往上冲击，击碎前额叶和头盖骨，毁坏额叶前皮质，估计一下这位仁兄的结果
+马上就没了
    ->failure
+在医院被抢救几周，没了
    ->failure
+除了眼睛瞎了，没有什么事
    ->failure
+竟然恢复了，不仅眼睛瞎了，还性情大变
     ->turn_to
     
=== LHS_Hard_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次的问题可是会变难了哦。
            ++n
            {index}. ->pick
    -2: 接下来的题可是会更加难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题，作为送分题……
            ++n
            {index}. ->General_Questions.pickSchool
    -4: 那么，下面一道，回归一下吧……
            ++n
            {index}. ->LHS_Easy_Questions.pick
    -5: 最后的最后！来点难题吧！
            ++n
            {index}. ->pick
    -6: 全部正确，辛苦你了，这些题对你来说可都不容易吧。
            ->success
}

=pick
{~->hard1|->hard2|->hard3|->hard4}

=hard1
下面那个活动调节是正反馈调节？
+女性生产时的子宫收缩
    ->turn_to
+体温调节
    ->failure
+正常情况下体内肾上腺素水平调节
    ->failure
+都是负反馈调节
    ->failure
     
=hard2
生物信息学是LHS的一个重要专业。在LHS的第一届学生大会上，来自BIM和BME的学生们欢聚一堂，享受了一顿自助餐。那么，生物信息学主要是哪几个学科的交叉学科？
+生物+计算机+统计
    ->turn_to
+物理+计算机+化学
    ->failure
+生物+电子+物理
    ->failure
+统计+电子+化学
    ->failure
    
=hard3
BME的学生们最喜欢实验了。他们试过在一个学期内上三个或以上的实验课。但是下面选项里面有一个他们（第一届学生）没有做的，那个实验课是：
+解剖实验
    ->turn_to
+电路设计实验
    ->failure
+化学实验
    ->failure
+物理实验
    ->failure

=hard4
LHS不时会举行聚餐，有吃的有喝的。你恰好想过去，那么LHS办公室在哪个地方：
+TC的4楼
    ->failure
+TD的5楼
    ->turn_to
+实验楼的3楼
    ->failure
+TC的5楼
    ->failure

/*************************************/
//              HSS Part             //
/*************************************/

=== HSS_Easy_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次可基本都是真真正正的学术问题了哦。
            ++n
            {index}. ->pick
    -2: 接下来可会慢慢变难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题作为放松：
            ++n
            {index}. ->General_Questions.pickEasy
    -4: 那么，下面一道，依然是和校园有关的：
            ++n
            {index}. ->General_Questions.pickHard
    -5: 看来你似乎觉得这些题都太简单了吗？那好啊，最后一道了，来点难题吧！
            ++n
            {index}. ->pick
            
    -6:     ->success
}

=pick
{~->easy1|->easy2|->easy3|->easy4|->easy5}

=easy1
“爷爷”是以下那种或哪些行为的代名词:
+A. 蓝色护眼文档底色
    ->failure
+B. “铲平”选题
    ->failure
+C. “你们能辨倒我，我才很开心”
    ->failure
+D. 以上全部
    ->turn_to

=easy2
人文学生常常因为什么受到其它学院的羡慕嫉妒恨？
+A. 超长假期
    ->turn_to
+B. 超高绩点
    ->failure
+C. 没有作业
    ->failure
+D. 没有考试
    ->failure
    
=easy3	
如果你是一名认真学习的人文学子，毕业前涉及的知识包括：
+A. 囚徒困境
    ->failure
+B. 晕轮效应
    ->failure
+C. 《第二性》
    ->failure
+D. 小孩子才做选择，我全都要（学）
    ->turn_to
    
=easy4	
人文学院名下的老师不包括：
+A. 运动健将体育教授
    ->failure
+B. 风度翩翩通识教授
    ->failure
+C. 博古通今语文教授
    ->failure
+D. 相貌堂堂经管教授
    ->turn_to
    
=easy5
以下哪个方向是人文学子研究生不能申请的：
+A. 教育
    ->failure
+B. 法律
    ->failure
+C. 新闻媒体
    ->failure
+D. 当然是只要想，各个方向都可以！
    ->turn_to
    
=== HSS_Hard_Questions ===
~index = index + 1
{index:
    -1: 准备好了吗，开始第一题咯。这次的问题可是会变难了哦。
            ++n
            {index}. ->pick
    -2: 接下来的题可是会更加难咯。
            ++n
            {index}. ->pick
    -3: 那么下面一道题，作为送分题……
            ++n
            {index}. ->General_Questions.pickEasy
    -4: 那么，下面一道，回归一下吧……
            ++n
            {index}. ->HSS_Easy_Questions.pick
    -5: 最后的最后！来点难题吧！
            ++n
            {index}. ->pick
    -6: 全部正确，辛苦你了，这些题对你来说可都不容易吧。
            ->success
}

=pick
{~->hard1|->hard2|->hard3|->hard4}

=hard1
人文同学常常挂在嘴边的“阿鸡”指的是什么？：
+A. 大吉大利，今晚吃鸡
    ->failure
+B. 反思日志（reflective journal）
    ->turn_to
+C. 一位不愿透露姓名教授的昵称
    ->failure

=hard2
人文学子专属“自习室”是哪里？：
+A. TA教学楼空教室
    ->failure
+B. SALL CENTRE 语言学习中心
    ->failure
+C. 口译室
    ->turn_to
+D. 教授办公室
    ->failure
    
=hard3
以下哪个电影名的翻译不是港版？：
+A. 优兽大都会
    ->failure
+B. 玩转脑朋友
    ->failure
+C. 可可夜总会
    ->turn_to
+D. 打爆互联网
    ->failure
    
=hard4	
人文学院的特产是：
+A. 生日会和教学楼里学长学姐的照片墙
    ->turn_to
+B. 年度茶会party
    ->failure
+C. 圣诞节专属小礼物
    ->failure


/*************************************/
//              Egg Part             //
/*************************************/

=== checkEgg ===
{egg == 0:再回去试试看吧}<>
{egg == "easy1.1": TC的楼上是什么呢？或许你可以去那里探查一下哦。}<>
{egg == "easy2.1":啊哦，LGU可不是官方英文简称哦，虽然学校里是有很多学生这么叫，但是官方简称可并不是LGU。 }<>
{egg == "easy2.2":你真的不是故意做错题的嘛……}<>
{egg == "easy3.1":如果真的有这么多学院也好啊，不过大概也不会有这么多专业吧。不管怎样，这道题你没有做对呢……虽然我觉得你可能是故意的，毕竟这么明显。}<>
{egg == "easy3.2":我们学校一个学院都没有的话，你在和哪个院的院长对话……}<>
{egg == "easy4.1":说起来，倒是的确有不少同学想住在逸夫，不过很可惜，就这个题而言，你还是做错了呢。}<>
{egg == "easy4.2":怎么会一个书院都没有呢……逸夫书院离这里也没几步远吧……}<>
{egg == "easy4.3":不好意思……装傻是不管用的，这题还是得算你错呢。}<>
{egg == "easy6.1":有志气！你真的准备修这么多学分嘛？不过可惜，起码这道题你是得不了分了……}<>
{egg == "easy7.1":能不能不要选这些明显被排除掉的选项嘛……}<>
{egg == "hard2.1":或许你可以实地考察一下，TA701有没有失物招领处……}<>
{egg == "hard2.2":或许你可以实地考察一下，TD130有没有失物招领处……}<>
{egg == "hard2.3":……你认真的嘛？希望你只是选着玩的，校长可是很忙的，可别随便打扰他……}<>
{egg == "hard4.1":如果这样的话，或许学生们会很开心吧……可惜你不会，因为这题你选错了。}<>
{egg == "hard5.1":起码也有一个离这里也没多远的潘多拉吧！不要故意选错误选项啊！}<>
{egg == "hard5.2":所以你是多算上了哪一个？全家福？信院？难不成……是会饮咖啡？可惜，它们都不是呢。}<>
{egg == "hard5.3":所以你是多算上了哪两个？全家福？信院？难不成……是会饮咖啡？可惜，它们都不是呢。}<>
+n
再回去多收集收集信息吧，下次再接再厉！
-
+n
->END

/*************************************/
//              Hint Part            //
/*************************************/

=== hint ===

=body
+n
整个问答一共分为<color=blue>三个阶段</color>，每个阶段会有若干道题。全部为<color=blue>单选</color>。难度上会有递进。
-
+n
因为是刚刚才接到消息，因此这套题是题库里临时随机组合出来的，有的时候，<color=blue>题目会有重复</color>，你可能会在不同阶段做到同一道题，有时，你甚至会在同一阶段做到同一道题。
-
+n
出题组成员也是五花八门，因此各种各样的题目都会出现。
-
+n
有的会考你对于学校某个设施或是某项规定的了解；有的会考你一些学院的常识；有的甚至会考你一些你不知道的学院内部轶事；当然，也会有几道单纯的学术问题……
-
+n
你或许会觉得很难，但是其实除了学术类问题需要完全靠你自己回答以外，<color=blue>其他的问题，你都可以在校园里找到答案</color>。
-
+n
当然，这就需要你认真地去搜集信息，打听情报。毕竟，逾期进入学院可不是那么容易的。
-
->->

=SME
->body->
+n
有关学术问题的话，其实这套题的难度对于新生来说，是有些大的。因此我不建议你直接上手去做。或许你可以尝试着预习一下大一的知识，再来尝试。
-
+n
或许你会觉得给一个新生出这样的题很过分，但其实在以后的大学学习中，你也会遇到很多以你那时的知识无法解决的问题。你只能<color=blue>靠自己学习</color>来一点点解决掉。
-
+n
<color=blue>自学</color>，对于大学生来说，是很重要的能力。尽早培养起自学习惯，对你来说没有坏处。
-
+n 
那么，等你准备好了，就来答题吧。
-
#notfinished
->DONE

=SSE
->body->
+n
有关学术问题的话……对于一个大一新生来说，唯一的难点，就在于编程的知识了吧。
-
+n
如果你要进入理工学院学习的话，可是要起码修两节编程的课程的。
-
+n
或许你会觉得给一个新生出这样的题很过分，但其实在以后的大学学习中，你也会遇到很多以你那时的知识无法解决的问题。你只能<color=blue>靠自己学习</color>来一点点解决掉。
-
+n
<color=blue>自学</color>，对于大学生来说，是很重要的能力。尽早培养起自学习惯，对你来说没有坏处。
-
+n 
那么，等你准备好了，就来答题吧。
-
#notfinished
->DONE

=HSS
->body->
+n
而且这套题基本没有考什么特别难的学术问题，更多的问题或许是有些让你摸不着头脑的类型。
-
+n
当你觉得有些困惑的时候，不妨去校园四处逛逛：去逛逛食堂、去看看风景，对了，你也可以去看看乐天一楼食堂前的那两只小猫。
-
+n
有的时候，答案不一定那么严肃，也可能会很诙谐，也可能会让你很意外。
-
+n 
总之，等你准备好了，就来答题吧。
-
#notfinished
->DONE

=LHS
->body->
+n
而且这套题基本没有考什么特别难的学术问题，更多的问题或许是有些让你摸不着头脑的类型。
-
+n
当你觉得有些困惑的时候，不妨去校园四处逛逛：去逛逛食堂、去看看风景，对了，你也可以去看看乐天一楼食堂前的那两只小猫。
-
+n
有的时候，答案不一定那么严肃，也可能会很诙谐，也可能会让你很意外。
-
+n 
总之，等你准备好了，就来答题吧。
-
#notfinished
->DONE

=SDS
->body->
+n
有关学术问题的话……对于一个大一新生来说，唯一的难点，就在于编程的知识了吧。
-
+n
如果你要进入理工学院学习的话，可是要起码修两节编程的课程的。
-
+n
或许你会觉得给一个新生出这样的题很过分，但其实在以后的大学学习中，你也会遇到很多以你那时的知识无法解决的问题。你只能<color=blue>靠自己学习</color>来一点点解决掉。
-
+n
<color=blue>自学</color>，对于大学生来说，是很重要的能力。尽早培养起自学习惯，对你来说没有坏处。
-
+n 
那么，等你准备好了，就来答题吧。
-
#notfinished
->DONE
// mission name: academic career
// #require: greetings of SSE
// #require: greetings of SME
// #require: greetings of HSS
// #require: greetings of LHS
// #require: greetings of SDS

VAR school = ""

=== attach_school_dean ===
{ school:
    - "SME": #attach: SME_dean
    - "SSE": #attach: SSE_dean
    - "LHS": #attach: LHS_dean
    - "HSS": #attach: HSS_dean
    - "SDS": #attach: SDS_dean
}
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
-
+n
是吗，那你准备选择哪个学院呢？
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
#upd_info
+n
好的，那么我会尽快为你办理好手续，让你进入该学院的。
-
+n
现在，去找你所在学院的院长聊聊吧，或许还会有一些别的事情要告诉你呢。
-
+n
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
因为你这个算是特殊情况，逾期进入学院，是特殊情况。因此我为你特殊安排了一场入院问答。
-
*入院考试？
因为是刚刚才接到消息，因此这套题是临时收集出来的，很多人都参与了出题，可是费了不少功夫。
-
+n
图书馆的前台人员；教务处的老师；你的学长学姐；<color=grey>甚至还有食堂前的那两只小猫……</color>
-
*小猫？
不要过分在意细节……总之这套题既不是硬核学术流，也不是日常轻松向。一些问题或许你目前也解决不了，一些问题或许你需要找别人打听才能得到答案。
-
+n
整个问答一共分为三个阶段，每个阶段会有若干道题。全部为<color=blue>单选</color>。难度上会有<color=blue>递进</color>。
-
+n
当然，作为平衡，我们也特殊允许你多次作答。
-
+n
那么，当你决定好了的话，就回到我这里来答题吧。
-
+n
->DONE

=== func_career_1 ===
#require: func_return_to_dean
-> attach_school_dean ->

*我决定好了，开始答题吧！
    ->General_Questions

=== func_career_2 ===
#require: func_career_1
-> attach_school_dean ->

*我准备好了，开始答题吧！
    {school == "SME":
    ->SME_Easy_Questions
    }{school == "SSE":
    ->SSE_Easy_Questions
    }{school == "LHS":
    ->LHS_Easy_Questions
    }{school == "HSS":
    ->HSS_Easy_Questions
    }{school == "SDS":
    ->SSE_Easy_Questions
    }
    

=== func_career_3 ===
#require: func_career_2
-> attach_school_dean ->

*我准备好了！
真的嘛，这可是最终的答题了哦，你确定自己准备好了吗？
    ++是的
    那么，开始吧！
        {school == "SME":
        ->SME_Hard_Questions
        }{school == "SSE":
        ->SSE_Hard_Questions
        }{school == "LHS":
        ->LHS_Hard_Questions
        }{school == "HSS":
        ->HSS_Hard_Questions
        }{school == "SDS":
        ->SSE_Hard_Questions
        }
    ++那我再准备准备吧
        #notfinished
        ->DONE

=== success ===
*全部正确
恭喜你！完成了第<>{!一|二|三}<>部分的题目。<>{!当你准备好之后，就可以来进行第二阶段的答题了！|当你准备好之后，就可以来进行最后的答题了！|那么恭喜你，你完成了所有的题目！->ending}

=== failure ===
{egg != "": ->checkEgg}
啊呀，你这道题答错了呢，再回去多加努力吧。
->DONE

=== ending ===
+n 
那么，你已经完成了所有的问题，恭喜你成为了<>
{school == "SME":经管学院}<>
{school == "SSE":理工学院}<>
{school == "LHS":生命健康学院}<>
{school == "HSS":人文学院}<>
{school == "SDS":数据科学院}<>
的一员！
-
+n 
这是给你的奖励。
-
+n 
TODO: 获取学院徽章（、学院皮肤？）
-
#endstory
->END

/*************************************/
//            General Part           //
/*************************************/

VAR egg = ""

=== General_Questions ===
+n
{!准备好开始了吗？|不要轻敌哦，刚开始题目简单是很正常的，接下来就会慢慢变难咯。|看来你似乎觉得这些题都太简单了吗？那好啊，来点难题吧！|不错嘛，这道题也能答得出来。|那么下面一道题……|接下来就是最后一道题咯……}
-
{!1. ->pickEasy|2. ->pickEasy|3. ->pickSchool|4. ->pickSchool|5. ->pickHard|6. ->pickHard|->success}

= pickEasy
{~->easy1|->easy2|->easy3|->easy4|->easy5}

= pickSchool
{school == "SME":{~->SME1|->SME2}}
{school == "SSE":{~->SSE1|->SSE2}}
{school == "LHS":{~->LHS1|->LHS2}}
{school == "HSS":{~->HSS1|->HSS2}}
{school == "SDS":{~->SDS1|->SDS2}}
->General_Questions
Error: NOT CHOSEN SCHOOL
-> DONE

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
        ->General_Questions
    + D. TA
        ~egg = "easy1.1"
        ->failure
    
= easy2
我校的官方英文简称是：
    + CUHK(SZ)
        ->General_Questions
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
        ->General_Questions
    + 0
        ~egg = "easy3.2"
        ->failure
        
= easy4
我校下园的书院有几个？
    + 天下大同，大家都在下园
        ~egg = 4.1
        ->failure
    + 全部木大，大家都不在下园
        ~egg = 4.2
        ->failure
    + 独步天下，只有逸夫独享下园
        ->General_Questions
    + 呆滞，我不知道
        ~egg = 4.3
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
        ->General_Questions
        
= easy6
学生至少需要修够___学分才能提前毕业：
    +120
        ->General_Questions
    +144 
        ->failure
    +180
        ->failure
    +1200
        ~egg = 6.1
        ->failure
        
= easy7
学生需要修够至少修够___门英语课，___门通识课：
    + 4 10
        ->failure
    + 0 0
        ~egg = 7.1
        ->failure
    + 4 4
        ->failure
    + 4 6
        ->General_Questions
        
/* General Questions: Hard Part */
= hard1
如果你是一名本科生，想要订图书馆的房间，下面哪一个不能订呢？
    +一楼的自习室
        ->failure
    +二楼的多人静音舱
        ->General_Questions
    +四楼的自习室
        ->failure
    
= hard2
如果你在丢了饭卡之后，想去失物招领处找回，应该去____。
    +RA楼下的物业处
        ->General_Questions
    +TA701的失物招领处
        ~egg = "hard2.1"
        ->failure
    +校长办公室
        ~egg = "hard2.2"
        ->failure
    +TD130的无人失物招领处
        ~egg = "hard2.1"
        ->failure
    
= hard3
我校一共有___个网球场，其中___个是室内的，___个是室外的。
    +5 3 2
        ->failure
    +4 4 0
        ->failure
    +5 5 0
        ->General_Questions
    +4 2 2 
        ->failure
    
= hard4
加退课一般在开学后的多长时间内？
    +两周
        ->General_Questions
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
        ->failure
    +5
        ->General_Questions
    +6
        ->failure
    +0 
        ~egg = "hard5.1"
        ->failure

/* General Questions: School Part */
// SME //
= SME1
我们学院的英文简称是？
    + SME
        ->General_Questions
    + SSE
        ->failure
    + HSS
        ->failure
    + LHS
        ->failure
    + SDS
        ->failure
        
= SME2
我们学院的英文全称是？
    + School of Management and Economics
        ->General_Questions
    + School of Science and Engineering
        ->failure
    + School of Humanities and Social Science
        ->failure
    + School of Life and Health Sciences
        ->failure
    + School of Data Science
        ->failure
        
// SSE //
= SSE1
我们学院的英文简称是？
    + SME
        ->failure
    + SSE
        ->General_Questions
    + HSS
        ->failure
    + LHS
        ->failure
    + SDS
        ->failure
        
= SSE2
我们学院的英文全称是？
    + School of Management and Economics
        ->failure
    + School of Science and Engineering
        ->General_Questions
    + School of Humanities and Social Science
        ->failure
    + School of Life and Health Sciences
        ->failure
    + School of Data Science
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
        ->General_Questions
    + SDS
        ->failure
        
= LHS2
我们学院的英文全称是？
    + School of Management and Economics
        ->failure
    + School of Science and Engineering
        ->failure
    + School of Humanities and Social Science
        ->General_Questions
    + School of Life and Health Sciences
        ->failure
    + School of Data Science
        ->failure
        
// HSS //
= HSS1
我们学院的英文简称是？
    + SME
        ->failure
    + SSE
        ->failure
    + HSS
        ->General_Questions
    + LHS
        ->failure
    + SDS
        ->failure
        
= HSS2
我们学院的英文全称是？
    + School of Management and Economics
        ->failure
    + School of Science and Engineering
        ->failure
    + School of Humanities and Social Science
        ->General_Questions
    + School of Life and Health Sciences
        ->failure
    + School of Data Science
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
        ->General_Questions

= SDS2
我们学院的英文全称是？
    + School of Management and Economics
        ->failure
    + School of Science and Engineering
        ->failure
    + School of Humanities and Social Science
        ->failure
    + School of Life and Health Sciences
        ->failure
    + School of Data Science
        ->General_Questions

/*************************************/
//              SME Part             //
/*************************************/

=== SME_Easy_Questions ===
{那么，让我们开始经管学院的答题吧！|很好，你答对了第一道题。|那么，下一题……|那么，最后一题……|全部正确，漂亮的回答！}
{!1. |2. |3. |4. |->success}<>
{~->easy1|->easy2|->easy3|->easy4|->easy5}

= easy1
一个变量遵循正态分布，均值为10，标准差为10。每一回抽取100个样本，抽取100回。请问这100回的平均值样本方差是多少？
    +A.0
        ->SME_Easy_Questions
    +B.1
        ->failure
    +C.0.5
        ->failure

= easy2
假设有一个假设检测，H0：均值参数为60。在使用计算器计算后我们得到一个双边检验的P值为0.0892。已知，Ha：均值参数小于60，z值为-1.7，alpha为0.05。是否可以拒绝原假设？
    +拒绝原假设H0
        ->SME_Easy_Questions
    +不拒绝原假设H0
        ->failure

= easy3
一支股票假定一直持续给股东每年2块的分红，当前年利率为2.5%，当前股票价格为100元。请问该股票是否值得购入？
    +是    
        ->failure
    +否
        ->SME_Easy_Questions
	

= easy4
假设在一个封闭独立的国家内，有一天突然出现一次生产上的技术革命，请问市场的均衡价格会有何变化？
    +提高    
        ->failure
    +降低
        ->SME_Easy_Questions
    +不变
        ->failure

= easy5
假设市场上有ABC三种商品，其中A、B互为替代品，B、C分别为normal good和inferior good，当A的价格下降时，B和C的消费有什么变化？
    +B消费下降，C不变    
        ->SME_Easy_Questions
    +B消费提高，C消费下降    
        ->failure
    +B消费下降，C消费提高
        ->failure

=== SME_Hard_Questions ===
{那么，让我们开始经管学院的答题吧。那么，第一道！|答对了，不过，之后可能就不会这么简单了。|最后一道了，准备好了吗？|你完成了所有的题目！}
{!1. |2. |3. |->success}<>
{~->hard1|->hard2|->hard3|->hard4}

= hard1 
假设有一个假设检测，H0：均值参数为50。在使用计算器计算后我们得到一个双边检验的P值为0.0124。已知，Ha：均值参数大于50，z值为-2.5，alpha为0.01。是否可以拒绝原假设？
    + A. 拒绝原假设H0
        ->failure
    + B.不拒绝原假设H0
        ->SME_Hard_Questions
    //（答案：不拒绝原假设H0-B）

= hard2 
假定有一家制造公司销售生产的渣渣，每年该公司生产5万吨渣渣，每吨卖4元，每吨的生产成本为2块5。这项项目的项目周期为3年，要求回报率为20%。该项目的固定成本是1万2每年，同时该公司投资了9万元到生产设备上。这些生产设备将在3年的项目周期结束后完全折旧。且该项目初期需要2万元的运营成本。税率为34%。请问项目周期的最后一年的现金流为多少？
    + A. 78,870
        ->failure
    + B. 77,780
        ->failure
    + C. 71,780
        ->SME_Hard_Questions
    //（答案：71,780-C）

= hard3
假设市场上有两家公司AB，它们共同面对一个线性需求曲线P(y)=a-by，边际成本恒定为C，先手为A，后手为B，请问这两个公司的Stackelberg equilibrium output是多少？
    + A. XA=(a-c)/b; XB=(a-c)/2b   
        ->failure
    + B. XA=(a-c)/2b; XB=(a-c)/4b   
        ->SME_Hard_Questions
    + C. XA=(a-c)/b; XB=(a-c)/3b
        ->failure
    //（答案：B）


= hard4
某位客户的效应方程是U(x,y,z)=x+(z^1/2)*f(y)
X是客户购买完别的生产物资剩下的钱，z是商品1，y是一种架子，当y>0时，f(y)=8; y=0时，f(y)=0，请问这客户会买多少个商品1？
    + A.12        
        ->failure
    + B.16        
        ->SME_Hard_Questions
    + C. 18
        ->failure
    //（答案：16-B）

/*************************************/
//              SSE Part             //
/*************************************/

=== SSE_Easy_Questions ===
+n
{那么，让我们开始理工学院的答题吧！|很好，你答对了第一道题。|那么，下一题……|那么，最后一题……|全部正确，漂亮的回答！}
-
{!1. |2. |3. |4. |->success}<>
{~->easy1|->easy2|->easy3|->easy4}

= easy1	
“ab”+”c”*2 结果是:
+abc2 
    ->failure
+abcabc 
    ->failure
+abcc
    ->SSE_Easy_Questions
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
    ->SSE_Easy_Questions

=easy3	
若地球卫星绕地球做匀速圆周运动，其实际绕行速率:
+ A.一定等于7.9km/s 
    ->failure
+ B.一定小于7.9km/s 
    ->SSE_Easy_Questions
+ C.一定大于7.9km/s 
    ->failure
+ D.介于7.9-11.2km/s之间 
    ->failure

=easy4 
关于地球同步通讯卫星的说法，正确的是: 
+ A.若其质量加倍，则轨道半径也要加倍 
    ->failure
+ B.它一定在赤道上空⻜行 
    ->failure
+ C.它以第一宇宙速度运行 
    ->failure
+ D.它运行的⻆速度与地球自转⻆速度相同 
    ->SSE_Easy_Questions

=== SSE_Hard_Questions ===
+n
{那么，让我们开始理工学院的答题吧。那么，第一道！|答对了，不过，之后可能就不会这么简单了。|最后一道了，准备好了吗？|你完成了所有的题目！}
-
{!1. |2. |3. |->success}<>
{~->hard1|->hard2|->hard3|->hard4}

=hard1 
导入模块的方式错误的是(D)
+ A. import mo
    ->failure
+ B. from mo import *
    ->failure
+ C. import mo as m
    ->failure
+ D. import m from mo
    ->SSE_Hard_Questions
    
=hard2	
分子式为C4H9Cl的同分异构体(不含对映异构)有( ) 
+ A. 2种 
    ->failure
+ B. 3种 
    ->failure
+ C. 4种 
    ->failure
+ D. 5种 
    ->SSE_Hard_Questions
    
=hard3 
下列哪个语句在Python中是非法的?( B ) 
+ A. x = y = z = 1 
    ->failure
+ B. x = (y = z + 1)
    ->SSE_Hard_Questions
+ C. x, y = y, x 
    ->failure
+ D. x += y
    ->failure

=hard4
下列物质一定与丁烯互为同系物的是: 
+ A.C2H4 
    ->SSE_Hard_Questions
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
+n
{那么，让我们开始理工学院的答题吧！|很好，你答对了第一道题。|那么，下一题……|那么，最后一题……|全部正确，漂亮的回答！}
-
{!1. |2. |3. |->success}<>
{~->easy1|->easy2|->easy3|->easy4}
=easy1
LHS作为小氪金学院，购置了许多新的实验室设施和装备。你猜猜下面哪个最贵？
+a) 5000只小白鼠
    ->failure
+b) 一台冷冻电镜
    ->LHS_Easy_Questions
+c) 3台三代测序仪
    ->failure
+d) 10台高分辨率质谱仪 
    ->failure
    
=easy2	
LHS（生命健康学院）有三个合作的诺奖研究院，别搜索，猜一下哪个诺奖获得者拿的是诺贝尔生理学或医学奖？
+a)	科比尔卡创新药物开发研究院的Prof. Brian K. KOBILKA
    ->failure
+b)	瓦谢尔计算生物研究院的Prof. Arieh Warshel
    ->failure
+c)	切哈诺沃精准和再生医学研究院的Prof. Aaron Ciechanover
    ->failure
+d)	虽然我们是生命健康学院，但是这奖雨我无瓜
    ->LHS_Easy_Questions
    
=easy3
从前有两个人，一个是正常人，一个是肾功能只剩下30%的人，他们吃相同的适量的食物。猜猜谁的尿液多？
+a)	肯定正常人啦
    ->failure
+b)	肾功能受损的
    ->failure
+c)	都一样
    ->LHS_Easy_Questions
+d)	看情况，abc都对
    ->failure
    
=easy4
    一位工人被一条铁棍以高速从下颚往上冲击，击碎前额叶和头盖骨，毁坏额叶前皮质，估计一下这位仁兄的结果
+a)	没了，马上就没了
    ->failure
+b)	在医院躺了被抢救几周，没了
    ->failure
+c)	竟然复原了，除了眼睛瞎了，没有什么事
    ->failure
+d)	竟然恢复了，不仅眼睛瞎了，还性情大变
     ->LHS_Easy_Questions
     
=== LHS_Hard_Questions ===
+n
{那么，让我们开始最后的答题吧。那么，第一道！|答对了，不过，之后可能就不会这么简单了。|最后一道了，准备好了吗？|你完成了所有的题目！}
-
{!1. |2. |3. |->success}<>
{~->hard1|->hard2|->hard3|->hard4}

=hard1
下面那个活动调节是正反馈调节？
+a)	女性生产时的子宫收缩
    ->LHS_Hard_Questions
+b)	体温调节
    ->failure
+c)	正常情况下体内肾上腺素水平调节
    ->failure
+d)	都是负反馈调节
    ->failure
     
=hard2
生物信息学是LHS的一个重要专业。在LHS的第一届学生大会上，来自BIM和BME的学生们欢聚一堂，享受了一顿自助餐。那么，生物信息学主要是哪几个学科的交叉学科？
+a)	生物+计算机+统计
    ->LHS_Hard_Questions
+b)	物理+计算机+化学
    ->failure
+c)	生物+电子+物理
    ->failure
+d)	统计+电子+化学
    ->failure
    
=hard3
BME的学生们最喜欢实验了。他们试过在一个学期内上三个或以上的实验课。但是下面选项里面有一个他们（第一届学生）没有做的，那个实验课是：
+a)	解剖实验
    ->LHS_Hard_Questions
+b)	电路设计实验
    ->failure
+c)	化学实验
    ->failure
+d)	物理实验
    ->failure

=hard4
LHS不时会举行聚餐，有吃的有喝的。你恰好想过去，那么LHS办公室在哪个地方：
+a)	TC的4楼
    ->failure
+b)	TD的5楼
    ->LHS_Hard_Questions
+c)	实验楼的3楼
    ->failure
+d)	TC的5楼
    ->failure

/*************************************/
//              HSS Part             //
/*************************************/

=== HSS_Easy_Questions ===

=easy1
“爷爷”是以下那种或哪些行为的代名词:
+A. 蓝色护眼文档底色
    ->failure
+B. “铲平”选题
    ->failure
+C. “你们能辨倒我，我才很开心”
    ->failure
+D. 以上全部
    ->HSS_Easy_Questions

=easy2
人文学生常常因为什么受到其它学院的羡慕嫉妒恨？
+A. 超长假期
    ->HSS_Easy_Questions
+B. 超高绩点
    ->failure
+C. 没有作业
    ->failure
+D. 没有考试
    ->failure
    
=esay3	
如果你是一名认真学习的人文学子，毕业前涉及的知识包括：
+A. 囚徒困境（Prisoner’s Dilemma）
    ->failure
+B. 晕轮效应（Halo Effect）
    ->failure
+C. 《第二性》（The Second Sex）
    ->failure
+D. 小孩子才做选择，我全都要（学）
    ->HSS_Easy_Questions
    
=easy4	
人文学院名下的老师不包括：
+A. 运动健将体育教授
    ->failure
+B. 风度翩翩通识教授
    ->failure
+C. 博古通今语文教授
    ->failure
+D. 相貌堂堂经管教授
    ->HSS_Easy_Questions
    
=easy5
以下哪个方向是人文学子研究生不能申请的：
+A. 教育
    ->failure
+B. 法律
    ->failure
+C. 新闻媒体
    ->failure
+D. 当然是只要想，各个方向都可以！
    ->HSS_Easy_Questions
    
=== HSS_Hard_Questions ===

=hard1
人文同学常常挂在嘴边的“阿鸡”指的是什么？：
+A. 大吉大利，今晚吃鸡
    ->failure
+B. 反思日志（reflective journal）
    ->HSS_Hard_Questions
+C. 一位不愿透露姓名教授的昵称
    ->failure

=hard2
人文学子专属“自习室”是哪里？：
+A. TA教学楼空教室
    ->failure
+B. SALL CENTRE 语言学习中心
    ->failure
+C. 口译室
    ->HSS_Hard_Questions
+D. 教授办公室
    ->failure
    
=hard3
以下哪个电影名的翻译不是港版？：
+A. 优兽大都会
    ->failure
+B. 玩转脑朋友
    ->failure
+C. 可可夜总会
    ->HSS_Hard_Questions
+D. 打爆互联网
    ->failure
    
=hard4	
人文学院的特产是：
+A. 生日会和教学楼里学长学姐的照片墙
    ->HSS_Hard_Questions
+B. 年度茶会party
    ->failure
+C. 圣诞节专属小礼物
    ->failure


/*************************************/
//              Egg Part             //
/*************************************/

=== checkEgg ===
+n
{egg == 0:再回去试试看吧}
{egg == "easy1.1": TC的楼上是什么呢？或许你可以去那里探查一下哦。}
{egg == "easy2.1":啊哦，LGU可不是官方英文简称哦，虽然学校里是有很多学生这么叫，但是官方简称可并不是LGU。 }
{egg == "easy2.2":你真的不是故意做错题的嘛……}
{egg == "easy3.1":这个题还不够明显的吗……}
{egg == "easy3.2":我们学校一个学院都没有的话，你在和哪个院的院长对话……}
{egg == "easy4.1":说起来，倒是的确有不少同学想住在逸夫，不过很可惜，就这个题而言，你还是做错了呢。}
{egg == "easy4.2":怎么会一个书院都没有呢……逸夫书院离这里也没几步远吧……}
{egg == "easy4.3":不好意思……拖时间是不管用的，不会的话就看看新生手册吧。}
{egg == "easy6.1":有志气！可惜这题选错了……}
{egg == "easy7.1":能不能不要选这些明显被排除掉的选项嘛……}

{egg == "hard2.1":或许你可以实地考察一下，TA701有没有失物招领处……}
{egg == "hard2.1":或许你可以实地考察一下，TD130有没有失物招领处……}
{egg == "hard2.2":……你认真的嘛？希望你只是选着玩的，校长可是很忙的，可别随便打扰他……}
{egg == "hard4.1":如果这样的话，或许学生们会很开心吧……可惜你不会，因为这题你选错了。}
{egg == "hard5.1":潘多拉离这里也没多远啊！不要故意选错误选项啊！}
-
+n
再回去多收集收集信息吧，下次再接再厉！
-
->DONE

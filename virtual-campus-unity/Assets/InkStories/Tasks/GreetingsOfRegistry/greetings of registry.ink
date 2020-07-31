//mission name: greetings of registry
#after: greetings of headmaster

#name: 教务处的问候
#description: 大学的学习可不像之前那么简单了，有很多细则都需要明确。去行政楼找教务处的老师为你讲解一下吧！

=== func_start ===
#override
#collidetrigger:registry_receptionist
你好，我是教务处的老师。
*你好，我是新来的学生
哦，是新生啊。你好你好，是从副校长那里过来的嘛。
-
+n 
那么，作为教务处的老师，我负责给你介绍一下我校学生在学术方面的一些规定守则。
-
+n
其实呢，这些信息大多在教务处官网都有。有些我甚至会直接原文读给你。
-
+n
但是呢，也有一些事项对于新生来说不太容易懂。因此对于这些事项，我也会单独拿出来解释的。
-
+n
那么，你想先了解哪个方面呢？
-
->introduction

=== introduction ===
*修业期限 -> introduction.duration
*选课及学分规定 ->introduction.lectures_credits
*加选及退课(add & drop) ->introduction.add_drop
*评核及考试 ->introduction.test_grade
* ->
    ++n 
    大学成绩的比拼，将不仅仅是知识单方面的比拼，也会有诸多其他方面的比拼，但最重要的，其实是<color=red>信息</color>。
    -
    +n
    如何获得更多的信息，如何合理利用有效信息，这些或许都是你要考虑的，以后就不仅仅是单单纯纯地闷头学习了。你或许已经成年或接近成年了，之后你的路，就很少有像以前的生活一样，一条路走到高考了。
    -
    +n 
    当然，再退一步讲，大学的比拼，也不仅仅是成绩的比拼，虽然成绩确实重要，但是对于一个大学学生来讲，找到自己目前想要做的，并为之奋斗，或许更加重要。
    -
    +n 
    好了，你现在对于整体的学术规定也有一些了解了，是该让你了解一下各个学院了。那么就让各个院长来为你介绍一下吧，他们现在应该在各个教学楼呢，让我看看……
    -
    +n
    ！
    -
    +n
    ！！
    -
    +n
    ！！！
    -
    +n
    他们怎么都不见了，都跑去哪了！
    -
    *<color=\#808080>（貌似刚刚教学楼就没有人啊……）</color>
    真是的，估计是因为刚开学。院长们喜欢在开学的时候在学校里晃荡，看看你们这些新来的学生，看看学校每年的新变化，毕竟平时他们都在忙学术，也没什么时间闲逛校园。
    -
    +n
    不好意思，估计得麻烦你帮我找下他们了，反正他们肯定不会待在教学楼附近，请你帮我把他们找回来吧。谢谢了！
    -
    +n
    大致的位置可能会有……让我想想……
    -
    +n
    有可能在<color=blue>图书馆</color>……<color=blue>每个食堂</color>都有可能……<color=blue>室内体育馆</color>也有可能……说不定也可能在<color=blue>东南门</color>那里……或者在<color=blue>小广场</color>……
    -
    +n
    这么一想，好像跟没说一样啊……
    -
    +n
    不管怎么样，你先去我上面说的几个地方看一下吧。真是不好意思，谢谢你了！
    -
    *好的
    #endstory
    ->END

= duration
+n
这一部分官网文件上写的已经很详尽了，我会挑重点念给你听：
-
+n 
“所有本科生的常规修业期限为<color=blue>四年</color>。学生预期于常规修业期限届满后毕业。
-
+n 
“学生可按特定程序及于指定期限内，以特定表格附陈合理原因向教务处处长申请，以更改修业期限。
-
+n 
“缩短常规修业期的申请须于预期毕业的学期最后一日<color=blue>前六个月</color>提出。
-
+n
“虽然可以缩短修业期，但是除非该生已在中大（深圳）修业<color=blue>至少达两年</color>，否则将不获毕业评核及推荐颁授学位。
-
+n 
“而于常规修业期限届满后延长修业期的申请，则须于常规修业期限最后修业年。
-
+n
“除获学院院务会推荐及教务处处长批准外，所有本科生的<color=blue>最长修业期限</color>为常规修业期限再加两年，即<color=blue>六年</color>，自学生首次注册日起计算。
-
+n
“此年限包括所有请假及休学等期间在内，但不包括学生所属国家规定强制服兵役的期间。对于休学创业的学生，其最长修业期限可根据国家相关规定和具体情况进行审批。
-
+n 
“最长修业期限届满后，未符合所有毕业规定的学生须自中大（深圳）退学，且不获颁发学位。
-
+n 
“开始前的特定申请期限内提出。于预期毕业的学期开始后才提出的申请，一般不会被接纳。”
-
->introduction

= lectures_credits
+n
这一部分的话，我先把文件上有关内容给你摘出来，遍念遍在中间做一些其他的补充。
-
+n 
“除根据程序获批准豁免之学分外，学生须修毕科目<color=red>一百二十学分</color>，始准毕业。”
-
+n
这里有关学分的问题，文件上省略了一些细节，或许你也在之前有听学长学姐提起过，我校的这个一百二十学分是怎样的结构。
-
+n
总的来说，分为五部分：
-
+n
<color=blue>大学核心课程(U-core)</color>
-
+n
<color=blue>大学核心课程(U-core)</color>
<color=blue>学院大礼包(school package)</color>
-
+n
<color=blue>大学核心课程(U-core)</color>
<color=blue>学院大礼包(school package)</color>
<color=blue>专业必修(major required)</color>
-
+n
<color=blue>大学核心课程(U-core)</color>
<color=blue>学院大礼包(school package)</color>
<color=blue>专业必修(major required)</color>
<color=blue>专业选修（major elective）</color>
-
+n
<color=blue>大学核心课程(U-core)</color>
<color=blue>学院大礼包(school package)</color>
<color=blue>专业必修(major required)</color>
<color=blue>专业选修（major elective）</color>
<color=blue>自由选修（free elective）</color>
-
+n
而为了方便起见，我们一般用英文简称来代指，分别是：
-
+n
<color=blue>大学核心课程(U-core)</color> <color=red>U-core</color>
<color=blue>学院大礼包(school package)</color> 
<color=blue>专业必修(major required)</color>   
<color=blue>专业选修（major elective）</color> 
<color=blue>自由选修（free elective）</color>  
-
+n
<color=blue>大学核心课程(U-core)</color> <color=red>U-core</color>
<color=blue>学院大礼包(school package)</color>   <color=red>Package</color>
<color=blue>专业必修(major required)</color>   
<color=blue>专业选修（major elective）</color> 
<color=blue>自由选修（free elective）</color>  
-
+n
<color=blue>大学核心课程(U-core)</color> <color=red>U-core</color>
<color=blue>学院大礼包(school package)</color>   <color=red>Package</color>
<color=blue>专业必修(major required)</color>   <color=red>MR</color>
<color=blue>专业选修（major elective）</color>  
<color=blue>自由选修（free elective）</color>   
-
+n
<color=blue>大学核心课程(U-core)</color> <color=red>U-core</color>
<color=blue>学院大礼包(school package)</color>   <color=red>Package</color>
<color=blue>专业必修(major required)</color>   <color=red>MR</color>
<color=blue>专业选修（major elective）</color>   <color=red>ME</color>
<color=blue>自由选修（free elective）</color>   
-
+n
<color=blue>大学核心课程(U-core)</color> <color=red>U-core</color>
<color=blue>学院大礼包(school package)</color>   <color=red>Package</color>
<color=blue>专业必修(major required)</color>   <color=red>MR</color>
<color=blue>专业选修（major elective）</color>   <color=red>ME</color>
<color=blue>自由选修（free elective）</color>   <color=red>Free</color>
-
+n 
<color=blue>大学核心课程</color>为每个中大（深圳）学生都必须修读的课程，主要包含4节英语课(12分)、6节通识课(18分)、1节中文课（3分）、2门体育课（2分）、1门IT课（1分）。一共<color=blue>36</color>学分。
-
+n 
<color=blue>学院大礼包</color>为每个学院的学生对应的必修课程。<color=blue>专业必修</color>、<color=blue>专业选修</color>，顾名思义，为学生升到大二选定专业后要开始学习的必修和选修科目。Package, MR, ME三部分学分统共<color=blue>72</color>学分
-
+n 
剩下的<color=blue>自由选修</color>部分占<color=blue>12</color>学分，学生可以自由选课，不受专业或学院限制。
-
+n 
所有的学分相加，即为文件中所提到的<color=red>120</color>学分。
-
+n 
“学生可报修一项或以上副修课程，并修毕该副修课程规定的学分。”
-
+n
但就目前来说，我校支持副修的学科不多，建议想要副修之前先到教务处办公室或向该专业教授咨询。
-
+n 
“学生于其常规修业期限内的每学期修课<color=blue>不得少于九学分</color>，亦<color=blue>不得多于十八学分</color>。学生于每一暑期修课<color=blue>不得多于六学分</color>，于每学年修课一概<color=blue>不得多于三十九学分</color>。
-
+n
然而其实如果你真的觉得18学分不够你学的，可以<color=red>提交加学分申请表</color>说明想要加到多少学分；教务处会根据你的成绩酌情批准。
-
+n
但是作为教务处的老师，我还是建议，如果你真的不是因为某些事情急着毕业急着修课、或者是对自己的学术水平充满信心，那么完全不必赶着刷学分。
-
+n
大学课程都是支持旁听的，如果你真的是对这门课有兴趣，不妨先旁听，再做决定。
-
+n
毕竟我们也见过不少递交加学分表时信心满满，期中之后又灰头土脸来申请退课的……
-
+n
如果你对自己的学业成绩真的很看重的话，那么我只能劝你，在选课时，切记慎重。
-
+n
“学生未获批准而在指定期限后未有选课，会被要求休学或被视作自动退学。学生选课多于规定的最高课业负荷，将按需要被要求退选超出的科目。
-
+n 
“学生如有任何必修科目不及格，须重修该科目或修读经有关主修课程核准的代替科目。学生可获安排补考由主修课程指定的科目，如补考不及格，则须重修科目。
-
+n 
“学生不得重修已修毕并获及格成绩的科目；惟按本学则规定必须重修者除外。”
-
->introduction

= add_drop
+n 
如何加退课或许是你们这些新生刚入学时最容易遇到的问题，尽管对于你们大一第一学年的新生来说，只用选择一门体育课就好了，但哪怕如此，也会出现很多问题。
-
+n 
技术上的操作问题，请到道远楼或发邮件找ITSO说明。我目前仅说明一些加课制度上的问题。
-
+n
在每学期开学前，教务处都会发送一封提示邮件，内含新学期的所有课程信息(包括授课内容，先修条件等信息)、加课时间及期限、以及一些特殊说明。学生须在规定期限内选定最少学分(一般为9学分)的课。
-
+n
<color=red>学生信息系统(Student Information System, SIS)</color>将会在几天内开放加课权限，但未进入加课期时，学生仅能将课程加入购物车。待到加课时间时才可开始正式加课。
-
+n
需注意，一门课可能会开不同的<color=blue>Lecture（讲课课程）</color>，不同的课程也会带有不同的<color=blue>Tutorial（练习课程）</color>。一般情况下，在开设了Tutorial时，学生需在选择Lecture的同时，选择一节Tutorial。
-
+n
课程信息方面需要注意的首先是<color=blue>课程时间</color>。每次开学前，大家都需要花一段时间排自己的课程表。而这个排表的过程，大部分往往就是在排时间。不同的修课时间之间哪怕在SIS上有1分钟的重叠（虽然正常情况下不会有），也没有办法通融。因此提前了解课程信息很重要。
-
+n
其次就是有关<color=blue>修读条件</color>不同的课程修读条件不尽相同，但大多数分为两类：专业和先修课程；这些都不难理解，相信也不需要解释。但同样，因为这些信息往往容易被忽视，造成很多学生在抢课时不明不白地就丢失了抢到课的机会。
-
+n 
“学生在学期授课开始后拟退选或加选科目，须按照学术委员会以及教务处不时决定的程序及期限办理。”
-
+n
一般在开学的<color=blue>前两周都是add&drop period</color>，这个时间学生可以在不违反以上规定地前提下自由加退课，目的是为了给学生们留出一个缓冲期。
-
+n
但是当过了add&drop period之后。将不能提出add或drop，学生或可以提出late drop，但将会在学生成绩表上做出标记。
-
+n
“凡学生退选科目而未按规定程序办理者，该科成绩作不及格计算。”
-
->introduction

= test_grade
+n
有关成绩的问题应该是每个学生都很关心的事情，但也是较难说明的一条，因为不同的课程评定成绩的方式不同。学生需在学期开始时仔细浏览该课程讲师给出的评分表。而我们在这里仅仅对部分信息和术语做一些强调说明。
-
+n 
每门课均有自己的学分，一般为1分-3分。学期结束时，学生会以字母的形式(A-F)得到每门学科的成绩。
-
+n 
评核及纪录课业成绩所使用的等级、标准及变换如下：
A :4.0   A-:3.7
B+:3.3   B :3.0   B-:2.7 
C+:2.3   C :2.0   C-:1.7 
D+:1.0   D :1.0   F :0.0
-
+n 
这是什么意思呢，比如你上了一门3分的英语课ENG1001，拿了A，那么这学期的这节ENG1001你的分数就是4.0了。
-
+n 
那么如何算学期末的总成绩呢？按照每门课的学分及成绩加权求平均。通俗的说，就是每门课的成绩×学分求和之后除以总学分。让我们举个例子。
-
+n 
比如这学期你上了MAT1001，ENG1001，PED1001，这三门课分别占3学分、3学分、1学分，最终你分别得到了A、A-、B+的成绩，那么你的最终成绩就是(3 x 4.0 + 3 x 3.7 + 1 x 3.3)/(3 + 3 + 1) = 3.7714。不错的成绩！
-
+n 
最后，让我们介绍几种不同的gpa。
-
+n 
<color=blue>tGPA</color>，即term GPA，为一学期结束对应的本学期GPA。
-
+n
<color=blue>mGPA</color>，即major GPA，为所有专业课的总GPA。
-
+n 
<color=blue>cGPA</color>，即cumulative GPA，为目前为止所有课程的总GPA。
-
+n 
一般情况下，我们在各项评定时所指的GPA都是cGPA，即目前为止的所有课程的总GPA。
->introduction

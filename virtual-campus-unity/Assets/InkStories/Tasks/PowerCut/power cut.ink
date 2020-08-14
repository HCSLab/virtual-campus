// mission name: power cut

#require: blackswan

#name: 停电事件
#description: 在寻找黑天鹅时，<color=red>逸夫宿管阿姨</color>似乎有一些别的想要让你帮忙的事情，或许是时候去找她问问看了。

=== func_start ===
#override
#collidetrigger: Shaw_dorm_admin
同学你可算是来了，我这边都忙不开手了！
*发生什么了？
逸夫书院大部分楼层都停电了，我这边一直在帮同学解决问题，一直有同学打电话过来询问电什么时候来……
-
+n
往常停电，很快就好了，但是这次不知道为什么，似乎很久都没有来电……
-
*需要我帮什么忙？
可能得麻烦你帮我去<color=red>TD旁边的物业处</color>问问，你之前应该去过吧。
-
*好的
#upd_description:逸夫书院停电了？受宿管阿姨的委托，你决定去<color=red>TD旁边的物业处</color>问问看是怎么一回事。
->DONE

=== func_Shaw_dorm_admin ===
#require: func_start
#override
#collidetrigger: Shaw_dorm_admin
麻烦你了，帮我去<color=red>TD旁边的物业处</color>问问有关停电的事好嘛。
+好的
->DONE

=== func_with_PM_staff ===
#require: func_start
#override
#collidetrigger: PM_staff
同学好，不好意思，我们这边有一些紧急情况要处理……
*是不是逸夫书院停电了？
啊，是的是的，是这样的，真的不好意思，我们会马上处理好的。
-
+n
啊呀……
-
*怎么了
我们的<color=blue>发电机钥匙</color>好像找不到了，丢到哪里去了……
-
+n
不会是丢到了<color=blue>学生活动中心和图书馆之间</color>下沉广场的草丛里了吧……我今天就只去过那里了。
-
+n
但是我没法离开啊，这里需要有一个人照看。
-
*要不我来帮你找吧
真的吗，那谢谢你了！
-
+n
不过那片草丛可有点暗啊，钥匙又那么小，你在黑暗中估计看不清的……你有<color=red>手电筒</color>吗？
-
*没有诶……
那怎么办……对了，你去找<color=red>宿管阿姨</color>问问，她肯定有<color=red>手电筒</color>！
-
*怎么又回去了……
#upd_description:根据物业处的描述，你需要回去找<color=red>逸夫的宿管老师借一个手电筒</color>，之后到<color=red>学生活动中心和图书馆之间</color>的下沉广场草丛去找找看。
->DONE

=== func_PM_staff ==
#require: func_with_PM_staff
#override
#collidetrigger:PM_staff
同学，你找到钥匙了吗？
+没有
#notfinished
->DONE

=== func_dark_area1 ===
#require: func_with_PM_staff
#without_global: item_手电筒
#collidetrigger: dark_area_collider
+n
你感觉这里似乎有些暗。
-
+n
你得先拿到手电筒，再来这里找钥匙。
-
+n 
#notfinished
->DONE

=== func_dialogue_with_dorm_admin ===
#require: func_with_PM_staff
#override
#collidetrigger: Shaw_dorm_admin
问的怎么样啦？
*如此这般……
哦……原来是这样啊。
-
*因此，我需要借一个手电筒
好嘞，这里有夜光的，头戴的，高亮的……
-
*就普通的就行……
那就这个吧。
-
+n
拿之前，得在这登记一下。用完别忘了还给我啊，到时候还得再登记呢。
-
*知道了，谢谢
#enable: engine_key
#additem: 手电筒
->DONE

=== func_founded ===
#require: func_dialogue_with_dorm_admin
#collidetrigger: engine_key
+n
你找到了钥匙。
-
+n
身前似乎有声音。你抬头望去。
-
+n
有一个身影出现了，你还没来得及看清ta的长相，这个身影就迅速消失了。
-
+n
你只能看到这个人似乎和你身材相仿，但是戴着一个面具。
-
+n
神秘的身影似乎跑向了<color=red>行政楼后山</color>的方向。
-
+n
你耸了耸肩，也没有想太多。大白天戴着面具，虽然很古怪，但也不违反校规，大概只是一个普通的同学吧。
-
+n
你把钥匙放到了兜里，准备赶去把它交给物业人员。
-
#additem: 发电机钥匙
#disable: engine_key
#upd_description:你在找钥匙的过程中看到了一个戴着面具的神秘人，ta跑向了行政楼后山方向。但既然找到了钥匙，还是先把钥匙还回到<color=red>物业处人员</color>那里吧。
->DONE

=== func_returning ===
#after:func_founded
#override
#collidetrigger: PM_staff
同学，你找到钥匙了吗？
*给，这是钥匙
谢谢，太感谢了！
-
*客气了
那么，现在就可以让技术人员去处理了，等会就可以来电了。
-
+n
你用的那个手电筒，是借宿管阿姨的吧，别忘了还回去啊。
-
*谢谢提醒
#upd_description:现在，把手电筒还给逸夫的<color=red>宿管阿姨</color>吧。
->DONE

=== func_ending ===
#after:func_returning
#attach: Shaw_dorm_admin
*我来还手电筒了，谢谢阿姨啦
哦？你刚刚来还过了啊。
-
*什么
对啊，你刚刚匆匆忙忙跑过来，慌慌张张把手电筒往我这一丢，登记完就走了。
-
+n
你看，这是你填的登记表啊。
-
*阅读登记表
<color=\#808080>（确实是我的笔迹啊……）</color>
-
+n
不过，你拿过来的这个手电筒，确实和之前你还过的那个挺像的……怎么会有两个一样的手电筒呢……？
-
+n
咦，不对，你现在新还的这个手电筒，虽然外形上确实很像，但是确实不是你之前找我借的。
-
+n
因为这个手电筒之前被某个同学借出去摔了一下，导致这个手电筒柄下面有裂痕的……
-
+n
可是，你现在给我的这个完全是全新的啊。
-
*……
这个手电筒，真的不是你的吗……？
-
*不是啊……
那就奇了怪了，好吧，我先收着，可能是哪里弄错了。
-
+n
<color=\#808080>（你想起了之前找钥匙时见过的那个人……）</color>
-
+n
<color=\#808080>（那个人似乎也拿着一个相似的手电筒……）</color>
-
+n
同学，还有什么事情嘛，没有的话，我就先去忙别的啦？
-
*好的
#upd_description:奇怪的是，宿管阿姨说你已经还过手电筒了，可是你明明才刚刚找到钥匙啊。你想起了之前见过的神秘人……
#endstory
->DONE

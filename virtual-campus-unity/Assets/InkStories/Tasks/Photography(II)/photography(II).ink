// photography(II) or more

#require: photography(I)

#name: 摄影入门（II）
#description: 继续摄影学习！这次的地点是……？别忘了，地点和角度可是照片的两大重点要素！


=== func_start ===
#attach: PM_staff
*我来领取下一个拍照任务啦
好的，是觉得自己准备好了吗。那么，这张就是你要<color=blue>复现</color>的照片了。
-
#enable: task(II)_img
+n
一定要注意拍摄的<color=blue>位置</color>和<color=blue>角度</color>。
-
+n
如果拍到足够满意的照片，就可以拿着照片回来找我咯。
-
+n 
老规矩，完成作业的话，我就会给你一张和之前一样难得一见的大师级超现实摄影作品。可以供你学习或者收藏哦。
-
*好的
#disable: task(II)_img
#addphoto: 照片模板02
->DONE

=== func_fail ===
#require:func_start
#without_global: photo_task(II)
#attach: PM_staff
+看，这些是我拍的照片
哎，没有我想要的那一张哦……似乎<color=blue>位置</color>和<color=blue>角度</color>有一点点偏差？
别泄气，再去试试吧。拍到合适的照片后屏幕上方会有提示哦~
-
+好吧
#notfinished
->DONE

=== func_end ===
#require:func_start
#require_photo: task(II)
#attach: PM_staff
*给，这是我拍的照片
哦，这张看起来不错嘛。
-
+n
好的，这次就算你通过咯。根据约定，这是你的奖励
-
#addphoto: 大师之作02
+n
怎么样，看着这张照片，是不是感觉自己还有待提升呢？
-
+n
相机不用急着还我，你可以拿去自己再练习练习。
-
+n
等到你觉得可以的时候，就可以来找我领取<color=blue>下一份作业</color>咯。
-
*知道了
#addskin: 芝加哥朋克
#endstory
->DONE

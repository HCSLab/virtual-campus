#after:find-bunny

===func_headmaster_ask_you===
#attach:headmaster
*   您能给我讲讲VR技术吗
-   VR这方面是蔡伟教授的专长，你去问问他，回来给我讲讲好吗：）
*   好的，我这就去
#enable:HoHo
->DONE

===func_ask_profCai===
#attach:profCai
#after:func_headmaster_ask_you
*   教授，什么是VR技术呀
-   VR就是<color=red>虚拟现实</color>。…………（讲了很多）
*   奥奥，我明白了
->DONE

===func_ask_HoHo===
#collidetrigger:HoHo
你好，有什么可以帮你的吗？
*   学长，VR是什么？
-   （假装这里有对话）
*   明白了，谢谢
#disable:HoHo
->DONE

===func_backto_headmaster===
#attach:headmaster
#after:func_ask_profCai
#after:func_ask_HoHo
*   我知道什么是VR技术了
-   你真棒。你知道吗，我们学校有VR体验室，你可以去体验一下。
*   太棒了，我这就去
#enable:VRlab-tutor
->DONE

===func_VRlab_tutor===
#collidetrigger:VRlab-tutor
你好，欢迎来到VR Lab！
*   我想体验一下VR
-   好的。（讲解用法）
*   明白了，开始吧
//#enable:VR
->DONE

===func_finish===
#collidetrigger:VRlab-tutor
#after:func_VRlab_tutor
送你一枚VR徽章！以后欢迎常来VR Lab玩！
*   谢谢
#endstory
->DONE
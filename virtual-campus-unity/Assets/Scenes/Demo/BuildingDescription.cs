using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDescription : MonoBehaviour
{
    public enum Building
	{
		SportsHall,
		Liwen,
		Letian,
		Zhiren,
		Chengdao,
		Zhixin,
		Daoyuan,
		ResearchBuilding,
		UnivLib,
		AdministrationBuilding,
		ShawInternationalExhibitionCenter,
		TA,
		TB,
		TC,
		TD,
		ShawCollege,
		StudentCenter
	};

	static readonly private string[] buildingNames =
	{
		"体育馆",
		"礼文楼",
		"乐天楼",
		"志仁楼",
		"诚道楼",
		"知新楼",
		"道远楼",
		"实验楼",
		"大学图书馆",
		"行政楼",
		"逸夫国际会议中心",
		"教学楼A栋",
		"教学楼B栋",
		"教学楼C栋",
		"教学楼D栋",
		"逸夫书院",
		"学生活动中心"
	};

	static readonly private string[] buildingDescriptions =
	{
		"我校还没有操场，因此室内体育馆就成了学生锻炼的不二之选。里面配备有游泳池、健身房、多功能室等等各种各样的设施。",
		"目前主要作为我校礼堂和考场，开学典礼以及其他许多集会都会在这里举行。闲时这里也供学生运动用。",
		"一楼用作食堂，二楼仍在装修，三楼是我校的大学创业中心。",
		"志仁楼和诚道楼曾经主要承担教学作用，但在新的教学楼建好之后，这两栋楼便主要用作教授们的办公和研究。有时也用于上课。",
		"志仁楼和诚道楼曾经主要承担教学作用，但在新的教学楼建好之后，这两栋楼便主要用作教授们的办公和研究。有时也用于上课。",
		"东翼是我们大学的旧图书馆，目前图书已经基本搬至新图书馆，因此主要供学生们自习用。共四层。西翼是许多教授办公的地方，也配备有一些实验室。",
		"我校部分行政部门办公地，大数据研究院也坐落在这里。一楼一般用于学术讲座以及贵宾接待。",
		"我校刚刚建好的实验楼(Research Buildings)。将来会成为我校许多实验室所在地。现在这两座实验楼都在等待设施搬入。其中实验楼B栋又名涂辉龙楼。",
		"我校的新图书馆，一般被称作新图。一至五楼对本科生开放，六楼仅对研究生开放。主要供学生自习及借阅书籍。",
		"行政楼采用双子楼设计，东座和西座在三层以上连接起来，因此其实在三层以上就没有东西座的概念了。担当着运转学校的作用，大多数校领导和学校部门的办公室都位于这里。",
		"逸夫国际会展中心是我校负责举办大型会议的地方，大多数由学校举办的校外会议都会在这里召开，有时也会用作校内活动场所。",
		"最主要的四栋教学楼(Teaching Buildings)，各自按ABCD命名。为了方便起见，分别被简称为TA，TB，TC以及TD。你现在所在的是TA，经管学院办公室就位于这里。",
		"最主要的四栋教学楼(Teaching Buildings)，各自按ABCD命名。为了方便起见，分别被简称为TA，TB，TC以及TD。你现在所在的是TB。",
		"最主要的四栋教学楼(Teaching Buildings)，各自按ABCD命名。为了方便起见，分别被简称为TA，TB，TC以及TD。你现在所在的是TC，人文学院办公室就位于这里。",
		"最主要的四栋教学楼(Teaching Buildings)，各自按ABCD命名。为了方便起见，分别被简称为TA，TB，TC以及TD。你现在所在的是TD，理工学院办公室就位于这里。",
		"书院是为学生们提供住宿的地方，是平时学生们起居的主要场所。书院楼内一般低层配备有齐全的功能房，中间生活层每层配备有卫生间和厨房，顶层配备有洗衣房及晾衣处。设施齐全，功能完备，力求为学生们做好生活保障。",
		"这里是学生活动中心。学生事务处、学生会的办公室都设在这里。同时由于配备有诸如钢琴房、舞蹈室等特定的功能房，这里也会时常成为一些社团经常活动的地点。"
	};

	[SerializeField]
	public Building buildingName;

	public string GetName()
	{
		return buildingNames[(int)buildingName];
	}

	public string GetDescription()
	{
		return buildingDescriptions[(int)buildingName];
	}
}

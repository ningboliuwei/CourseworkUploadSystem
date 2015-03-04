using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CourseworkInfo 的摘要说明
/// </summary>
public class CourseworkInfo
{
	//ID
	public string ID { get; set; }
	//作业名称
	public string Name { get; set; }

	//作业发布时间
	public DateTime PublishTime { get; set; }

	//最长允许天数
	public int DaysBeforeDeadline { get; set; }
}
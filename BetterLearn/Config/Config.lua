return {
	Title = "突破优化",
	Author = "liuguang",
	Cover = "cover.png",
	GameVersion = "0.0.64.35",
	Version = "1.0.0.1",
	TagList = {
		"Modifications"
	},
	BackendPlugins = 
	{
		[1] = "BetterLearn.dll"
	},
--	FileId = 3004643355,
--	Source = 1,
	Description = [[
优化突破概率,大部分格子可以100%成功。
突破不消耗次数(可选择是否开启)
修习一键100%(可选择是否开启)
突破格重排(可选择是否开启)
]],
	DefaultSettings = 
	{
		[1] = 
		{
			SettingType = "Toggle",
			DisplayName = "突破不消耗次数",
			Key = "BreakNotCostedStep",
			DefaultValue = false
		},
		[2] = 
		{
			SettingType = "Toggle",
			DisplayName = "修习一键100%",
			Key = "FastPractice",
			DefaultValue = false
		},
		[3] = 
		{
			SettingType = "Toggle",
			DisplayName = "突破格重排",
			Description = "将金色格子移动到更方便连接的位置,不随机分布",
			Key = "ResetBreakPlate",
			DefaultValue = false
		}
	}
}
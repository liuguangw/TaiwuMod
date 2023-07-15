return {
	Title = "突破优化",
	BackendPlugins = 
	{
		[1] = "BetterLearn.dll"
	},
	Author = "liuguang",
	Cover = "cover.png",
--	FileId = 3004643355,
	Source = 1,
	Description = [[
优化突破概率,大部分格子可以100%成功。
突破不消耗次数(可选择是否开启)
修习一键100%(可选择是否开启)
	]],
	GameVersion = "0.0.63.42",
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
		}
	}
}
return {
	Title = "制造优化",
	BackendPlugins = 
	{
		[1] = "BetterMake.dll"
	},
	Author = "liuguang",
	Cover = "cover.png",
--	FileId = 3011575950,
	Source = 1,
	Description = [[
降低制造时的造诣要求(可选择是否开启)
无需额外建筑，也能制作更好的物品(可选择是否开启)
	]],
	GameVersion = "0.0.63.42",
	DefaultSettings = 
	{
		[1] = 
		{
			SettingType = "Toggle",
			DisplayName = "降低造诣要求",
			Key = "AttainmentRequirementReduce",
			DefaultValue = false
		},
		[2] = 
		{
			SettingType = "Toggle",
			DisplayName = "无需额外建筑",
			Key = "UpgradeMakeItem",
			DefaultValue = false
		}
	}
}
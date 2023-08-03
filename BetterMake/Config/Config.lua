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
以下均可按需要开启或者关闭
降低制造时的造诣要求
无需额外建筑，也能制作更好的物品
太吾村额外建造空间，增加额外的建筑空间上限
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
		},
		[3] = {
			SettingType = "Slider",
			DisplayName = "太吾村额外建造空间",
			Key = "BuildingSpaceExtra",
			DefaultValue = 0,
			MinValue = 0,
			MaxValue = 200,
			Description = "增加太吾村的额外建筑空间上限,为0时保持原版",
		},
	}
}
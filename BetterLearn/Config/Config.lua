return {
	Title = "突破优化",
	Author = "liuguang",
	Cover = "cover.png",
	GameVersion = "0.0.64.35",
	Version = "1.0.0.3",
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
显示所有格子,并优化突破概率,大部分格子可以100%成功.
这个mod不会修改金色格子的类型和数量,默认情况下格子的位置也不会改变,除非开启突破格重排选项.
如果开启了突破格重排,那么金色格子会被移动到更方便连接的位置,而不会随机分布.

以下功能可以根据需要来选择开启或者关闭:
优化突破概率
突破不消耗次数
修习一键100%
突破格重排
]],
	DefaultSettings = 
	{
		{
			SettingType = "Toggle",
			DisplayName = "优化优化概率",
			Description = "提高突破成功的概率",
			Key = "BetterRate",
			DefaultValue = true
		},
		{
			SettingType = "Toggle",
			DisplayName = "突破不消耗次数",
			Key = "BreakNotCostedStep",
			DefaultValue = false
		},
		{
			SettingType = "Toggle",
			DisplayName = "修习一键100%",
			Key = "FastPractice",
			DefaultValue = false
		},
		{
			SettingType = "Toggle",
			DisplayName = "突破格重排",
			Description = "将金色格子移动到更方便连接的位置,不随机分布",
			Key = "ResetBreakPlate",
			DefaultValue = false
		}
	}
}
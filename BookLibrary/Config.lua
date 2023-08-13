return {
	Cover = "Cover.png",
	DefaultSettings = {
		[1] = {
			SettingType = "InputField",
			Description = "修改此按键以启动修改器界面",
			DisplayName = "启动按键",
			Key = "HotKeyText",
			DefaultValue = "F5",
		},
	},
	Title = "太吾出版社",
	Author = "流光",
	Description = [[
这是一个可以获取书籍的工具，游戏里的任何书籍都可以获取。
可以自定义所需要的总纲、正逆篇。
通过此工具获得的书籍不会有残页、亡佚页，绝对的完整。书籍的最大耐久度也是游戏中所能达到的最大耐久。
有了这个工具，获取神一品书籍再也不难了(原游戏里面高品书籍不仅少，而且还残缺不全)。
默认快捷键为F5，适度使用，否则游戏乐趣可能大幅降低。
]],
	HasArchive = false,
	FrontendPlugins = {
		[1] = "BookLibrary.dll",
	},
	BackendPlugins = {
		[1] = "BookLibraryBackend.dll",
	},
	GameVersion = "0.0.64.35",
	Version = "1.0.0.0",
	TagList = {
		"Modifications"
	},
--	Source = 1,
--	FileId = 3019439426,
}

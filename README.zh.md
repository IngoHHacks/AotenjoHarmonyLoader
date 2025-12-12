# 青天井的Harmony加载器

*注：我不是中文母语者，所以本节的翻译可能非常糟糕，可能会因为滑稽的误译而让您哭泣。如果您是中文母语者并且愿意帮助改进翻译，请随时提交拉取请求或直接与我联系。感谢您的理解！*

## 介紹

青天井的官方模組加載器使用Lua。雖然使用Lua創建模組很簡單，但對於想要修改遊戲核心功能或實現複雜功能的開發者來說，可能會有一些限制。Lua和C#之間的互操作性也可能因數據類型轉換、索引差異、空值處理和額外開銷而變得麻煩。  
青天井的Harmony加載器是一個模組加載器，允許您使用C#和Harmony庫創建模組。Harmony是一個流行的庫，用於在運行時修補.NET應用程序，使您能夠修改現有方法、添加新功能並更改遊戲行為，而無需覆蓋原始代碼。  
與官方模組加載器一樣，Harmony加載器支持同時加載多個模組。然而，它的穩定性較差，可能會因遊戲更新而中斷，因此請自行承擔風險。建議盡可能使用官方模組API，以獲得更好的兼容性和穩定性。

## 使用方法

首先，按照[官方文檔](https://github.com/XOCatGames/Aotenjo-Src/wiki/1.-Getting-Started-%E5%BF%AB%E9%80%9F%E4%B8%8A%E6%89%8B)中的說明，像平常一樣使用Lua在青天井中設置模組。一旦您準備好模組結構，請按照以下步驟將Harmony加載器集成進去：
- 確保您已在青天井中安裝了Harmony加載器模組（通過Steam工作坊或手動安裝）。
- 在您喜歡的IDE（例如Visual Studio、JetBrains Rider）中創建一個新的C#類庫項目。
    - 建議目標為.NET Framework 4.7.2，以確保兼容性。
- 將Harmony庫、此庫、青天井的程序集以及您的模組所需的任何其他依賴項添加為引用。
    - 除了上述三個程序集之外，您的模組所依賴的任何程序集都必須放置在模組目錄的`libs`文件夾中。
    - 建議對青天井程序集進行剝離和公開。請參閱參考部分以獲取可以幫助您完成此操作的工具。
    - 在訪問剝離程序集中的私有成員時，可能需要在項目設置中啟用AllowUnsafeBlocks選項。
- 創建一個繼承自`HarmonyLoaderAotenjo.HarmonyMod`的類。
    - 重寫`ModName`、`ModAuthor`和`ModVersion`屬性，以提供有關您的模組的元數據。
    - `Init()`方法在加載模組時調用。
    - 當模組加載時，補丁會自動應用，因此您不需要手動調用`harmony.PatchAll()`。
- 使用Harmony補丁和其他C#功能實現您的模組邏輯。
- 構建您的項目以生成DLL文件。
- 將編譯好的DLL放入模組目錄中的`harmony`文件夾中。
- 啟動青天井，當您的模組加載時，您的Harmony補丁應該會自動應用。

**建議盡可能使用官方模組API（遊戲程序集中的`Aotenjo`命名空間），以獲得更好的兼容性和穩定性，因為該API旨在跨更新保持穩定。當官方API無法提供所需功能時，應使用Harmony補丁。**

## 文件結構
您的模組目錄應如下所示：

```
MyHarmonyMod/
├── scripts/                  # Lua腳本文件夾（如果有的話）
│   └── my_script.lua         # 示例Lua腳本
├── harmony/                  # Harmony DLL文件夾
│   └── MyHarmonyMod.dll      # 您編譯的Harmony模組DLL
├── libs/                     # 額外依賴項文件夾（如果有的話）
│   └── SomeDependency.dll    # 示例依賴項DLL
├── modinfo.json              # 模組元數據文件
└── other_mod_files...        # 您的模組所需的任何其他文件/文件夾（例如紋理）
```

## 參考資料

### 青天井模組文檔

[GitHub维基](https://github.com/XOCatGames/Aotenjo-Src/wiki/1.-Getting-Started-%E5%BF%AB%E9%80%9F%E4%B8%8A%E6%89%8B)

### Harmony文檔
[Harmony介紹](https://harmony.pardeike.net/articles/intro.html)

### 遊戲源代碼
[GitHub](https://github.com/XOCatGames/Aotenjo-Src)
青天井的源代碼在GitHub上公開可用。您可以參考它以了解遊戲的架構並查找要修補的類和方法。  
或者，您可以使用ILSpy、dnSpy或其他.NET反編譯器來探索遊戲的程序集。

### 公開和剝離工具
[Publicize](https://github.com/jacobEAdamson/publicize/releases/tag/v1.0.0)  
[AssemblyPublicizer](https://github.com/BepInEx/BepInEx.AssemblyPublicizer)  
[NStrip](https://github.com/bbepis/NStrip)

## 麻將小知識
你知道嗎？青天井在日語中意為“藍色天花板”。
這是日本麻將中的一種規則，分數沒有上限，允許非常高分的牌型。
“藍色”象徵著天空，描繪了天空的無限特性。
我相信你可以猜到這款遊戲為什麼會被命名為這個名字！

## 許可證
https://creativecommons.org/publicdomain/zero/1.0/  
本作品採用創用CC0 1.0通用許可證授權。  
您可以自由的複製、修改、分發和執行該作品，即使是用於商業目的，也無需徵得許可。  
不提供任何保證或責任。如果出現問題，那是您的技能問題。  
有關更多信息，請參閱上述鏈接中的許可證文本。

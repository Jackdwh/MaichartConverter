# `CompileDatabase`命令指南

### 转译整个含Ma2文件的目录

## 基础语法

    MaichartConverter CompileDatabase --参数

## 示例用法

    MaichartConverter CompileDatabase -p "/Users/Neskol/MaiAnalysis/A000/" -c "/Users/Neskol/MaiAnalysis/Image/Texture2D/" -v "/Users/Neskol/MaiAnalysis/DXBGA_HEVC/" -m "/Users/Neskol/MaiAnalysis/Sound/" -o "/Users/Neskol/MaiAnalysis/Output_HEVC_By_Cab/" -g 2 -d

## 可用参数

### 必要参数

> 除非另有所指，`XXXXXX`在此指代谱面ID

- `-p | --path <目录>`: `A000`目录的路径。需要该文件夹有如下结构：`A000/music/musicXXXXXX/Music.xml`
- `-o | --output <目录>`: 输出文件夹路径。如果该路径不存在，程序会尝试在该位置创建新文件夹。请确保该路径可写。

### 可选参数

- `-m | --music <目录>`: MP3文件夹的路径。需要该文件夹下文件的命名方式为： `musicXXXXXX.mp3`.
- `-c | --cover <目录>`: 曲目插图文件夹的路径。需要该文件夹下文件的命名方式为： `UI_Jacket_XXXXXX.png`.
- `-v | --video <目录>`: 背景视频文件夹的路径。需要该文件夹下文件的命名方式为： `XXXXXX.mp4`
- `-g | --genre <数字 0-6> `: 输出文件夹分类方式。
  `0 = 曲目类别, 1 = 曲目难度, 2 = 初出版本, 3 = 曲目作者, 4 = BPM, 5 - SD/DX谱面, 6 = 不创建子文件夹`
- `-d | --decimal`: 使程序使用定数作为显示的难度 (例如： `14+ => 14.7`).
- `-i | --ignore`: 忽略缺失的素材并继续转换。缺失素材的曲目将以`_incomplete`结尾，具体缺失的素材将会在`log.txt`里列出。
- `-n | --number`: 使用 `musicID` 作为文件夹名。
- `-j | --json`: 额外创建一个JSON格式的日志文件(`index.json`)。
- `-z | --zip`: 压缩每个转换后的文件夹，并在压缩完成后删除对应的文件夹。
- `-k | --collection`: 选择导出manifest.json，会在输出文件夹生成对应的collections文件夹，可以启用收藏夹功能

### 弃用/开发中参数:

> 以下参数没有完成开发，有条件的话还请实验/测试一下

- `-f | --format <选项>`: 强制将谱面编写为该格式。 (仅限 `maidata.txt`内). 可用格式: `Simai, SimaiFes, Ma2, Ma2_104`.
- `-r | --rotate <选项>`:
  强制按要求旋转谱面。可用选项：
  `Clockwise90, Clockwise180, CounterClockwise90, CounterClockwise180, UpsideDown, LeftToRight`.
- `-s | --shift <整数>`: 将所有谱面按指定间隔向前/向后平移。

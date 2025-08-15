# showSimulinkVer

## 概要

showSimulinkVerは、.NET 8で開発されたアプリケーションである。Windows上でのみ動作する。
目的はSimulinkのバージョン・作成日時・更新日時・最終更新者の情報をMessageBoxで表示する。

## 必要環境

- .NET 8 SDK
- 対応OS: Windows

## インストール

Release からインストーラをダウンロードしてインストールする。
通常はmsiファイルだけで良い。

## 使い方

1. インストール後、`showSimulinkVer.exe` のショートカットを「送る」メニューに登録する。  
   `%APPDATA%\Microsoft\Windows\SendTo` フォルダにショートカットを置くと登録できる。
2. `.slx` または `.mdl` ファイルを右クリック → 「送る」→ `showSimulinkVer` を選択。
3. MATLAB Version・作成日時・更新日時・最終編集者が MessageBox に表示される。

## おまけ: なぜMATLABプログラムではないのか

Simulinkモデルなのだから、MATLABプログラムであるほうが美しいと感じるかも知れない。
確かにMATLABスクリプトでもバージョン情報は取得できる。またアプリ化もできるだろう。

しかしMATLABのアプリは起動に時間を要する。モデルのバージョンはさっさと確認したいものだ。
また、アプリではないMATLABスクリプトはMATLABを起動しないと実行できない。
もしも調べたモデルのバージョンが違うと別のMATLABを起動しなおす羽目になる。

上記のような理由で、軽量・即起動・MATLABに依存しないで情報を確認できるようにC#でアプリを作成した。

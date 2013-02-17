@echo off
pushd ..\release
call make_release.cmd
popd

del *.exe 2> nul
del *.dll 2> nul
echo F | xcopy /y /d ..\source\TimSort.Tests.App\bin\Release\TimSort.Tests.App.exe test.exe
xcopy /y /d ..\release\unsafe\TimSort.dll .\

:loop
rem SuiteId: test group id (file name for summary file
rem TestId: test id (summary file will be sorted by this field)
rem Algorithm: QuickSort|TimSort
rem Containder: Array|List|IList|SlowList
rem Key: Int32|Guid|String|Comparable|NonComparable
rem Sort: Ascending|Ascending80|Random|Descending80|Descending
rem Length: number of elements

goto :skip

rem ---- Int32|Array
test.exe std 01 QuickSort Array Int32 Ascending 10000000
test.exe std 02 TimSort Array Int32 Ascending 10000000
test.exe std 03 QuickSort Array Int32 Ascending80 10000000
test.exe std 04 TimSort Array Int32 Ascending80 10000000
test.exe std 05 QuickSort Array Int32 Random 10000000
test.exe std 06 TimSort Array Int32 Random 10000000
test.exe std 07 QuickSort Array Int32 Descending80 10000000
test.exe std 08 TimSort Array Int32 Descending80 10000000
test.exe std 09 QuickSort Array Int32 Descending 10000000
test.exe std 10 TimSort Array Int32 Descending 10000000

rem ---- Guid|Array
test.exe std 11 QuickSort Array Guid Ascending 10000000
test.exe std 12 TimSort Array Guid Ascending 10000000
test.exe std 13 QuickSort Array Guid Ascending80 10000000
test.exe std 14 TimSort Array Guid Ascending80 10000000
test.exe std 15 QuickSort Array Guid Random 10000000
test.exe std 16 TimSort Array Guid Random 10000000
test.exe std 17 QuickSort Array Guid Descending80 10000000
test.exe std 18 TimSort Array Guid Descending80 10000000
test.exe std 19 QuickSort Array Guid Descending 10000000
test.exe std 20 TimSort Array Guid Descending 10000000

rem ---- String|Array
test.exe std 21 QuickSort Array String Ascending 10000000
test.exe std 22 TimSort Array String Ascending 10000000
test.exe std 23 QuickSort Array String Ascending80 10000000
test.exe std 24 TimSort Array String Ascending80 10000000
test.exe std 25 QuickSort Array String Random 10000000
test.exe std 26 TimSort Array String Random 10000000
test.exe std 27 QuickSort Array String Descending80 10000000
test.exe std 28 TimSort Array String Descending80 10000000
test.exe std 29 QuickSort Array String Descending 10000000
test.exe std 30 TimSort Array String Descending 10000000

:skip
rem ---- Comparable|Array
test.exe std 31 QuickSort Array Comparable Ascending 10000000
test.exe std 32 TimSort Array Comparable Ascending 10000000
test.exe std 33 QuickSort Array Comparable Ascending80 10000000
test.exe std 34 TimSort Array Comparable Ascending80 10000000
test.exe std 35 QuickSort Array Comparable Random 10000000
test.exe std 36 TimSort Array Comparable Random 10000000
test.exe std 37 QuickSort Array Comparable Descending80 10000000
test.exe std 38 TimSort Array Comparable Descending80 10000000
test.exe std 39 QuickSort Array Comparable Descending 10000000
test.exe std 40 TimSort Array Comparable Descending 10000000

rem ---- NonComparable|Array
test.exe std 41 QuickSort Array NonComparable Ascending 10000000
test.exe std 42 TimSort Array NonComparable Ascending 10000000
test.exe std 43 QuickSort Array NonComparable Ascending80 10000000
test.exe std 44 TimSort Array NonComparable Ascending80 10000000
test.exe std 45 QuickSort Array NonComparable Random 10000000
test.exe std 46 TimSort Array NonComparable Random 10000000
test.exe std 47 QuickSort Array NonComparable Descending80 10000000
test.exe std 48 TimSort Array NonComparable Descending80 10000000
test.exe std 49 QuickSort Array NonComparable Descending 10000000
test.exe std 50 TimSort Array NonComparable Descending 10000000

goto :loop
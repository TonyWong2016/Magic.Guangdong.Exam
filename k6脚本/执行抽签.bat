@echo off
setlocal

rem 设置Python解释器的路径，如果是Anaconda环境，需要指定完整路径
set PYTHON_PATH=python

rem 检查Python是否可用
%PYTHON_PATH% -V >nul 2>&1
if errorlevel 1 (
    echo Python not found. Please ensure Python is installed and accessible from your PATH.
    exit /b 1
)

rem 安装必要的Python库
%PYTHON_PATH% -m pip install --upgrade pip
%PYTHON_PATH% -m pip install pandas openpyxl

rem 设置Excel文件的路径
set EXCEL_FILE_PATH=%1

rem 检查是否提供了Excel文件路径
if "%EXCEL_FILE_PATH%"=="" (
    echo No Excel file path provided.
    exit /b 1
)

rem 调用Python脚本并传递Excel文件路径作为参数
%PYTHON_PATH% lottery.py %EXCEL_FILE_PATH%

endlocal
import pandas as pd
import random
import time
import argparse

def load_names_from_excel(file_path):
    # 使用pandas读取Excel文件
    df = pd.read_excel(file_path)
    # 假设名字位于第一列
    names = df.iloc[:, 0].tolist()
    return names

def draw_lottery(names):
    print("抽签开始...")
    print("倒计时开始：")

    # 倒计时5秒，每秒随机闪现10个名字
    for remaining_time in range(5, 0, -1):
        for _ in range(10):
            selected_name = random.choice(names)
            print(f"\r{selected_name}", end="")
            time.sleep(0.1)  # 每个名字显示0.1秒
        print(f"\r{remaining_time} 秒...", end="")
        time.sleep(0.1)  # 额外的0.1秒等待，确保每秒结束时显示剩余时间

   # print("\r倒计时结束！")  # 清除最后一行的名字
    print("最终结果是：")
    final_name = random.choice(names)
    print(final_name)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="从Excel文件中随机抽取一个名字")
    parser.add_argument("file_path", type=str, help="Excel文件的路径")
    args = parser.parse_args()

    file_path = args.file_path
    names = load_names_from_excel(file_path)
    draw_lottery(names)
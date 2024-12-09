import pandas as pd
import re
import requests

def process_text_and_extract_images(text):
    """
    处理文本（去除HTML标签）并提取图片链接
    """
    clean_text = re.sub('<[^>]*>', '', text)
    img_urls = re.findall('src="([^"]+)"', text)
    return clean_text, img_urls

def download_images(img_urls, prefix):
    """
    根据图片链接下载图片，保存到以prefix命名的文件夹下，并返回图片文件名列表
    """
    downloaded_images = []
    for index, url in enumerate(img_urls):
        try:
            response = requests.get(url)
            if response.status_code == 200:
                img_name = f"{prefix}_{index + 1}.jpg"  # 根据需要调整文件名格式
                with open(f'{prefix}_images/{img_name}', 'wb') as f:
                    f.write(response.content)
                downloaded_images.append(img_name)
                print(f"已成功下载图片: {img_name}")
        except Exception as e:
            print(f"下载图片 {url} 出错: {e}")
    return downloaded_images

# 读取原始Excel文件
data = pd.read_excel('original_file.xlsx')  # 替换成真实的原始Excel文件名

# 处理题目列
question_texts = []
question_images = []
for index, row in data.iterrows():
    question_text, question_img_urls = process_text_and_extract_images(row['question'])
    question_texts.append(question_text)
    downloaded_question_images = download_images(question_img_urls, f"question_{index}")
    question_images.append(downloaded_question_images)

# 处理选项列（假设选项列以逗号分隔多个选项文本，你可以按实际修改逻辑）
option_texts = []
option_images = []
for index, row in data.iterrows():
    all_option_texts = []
    all_option_images = []
    options = row['options'].split(',')
    for option in options:
        option_text, option_img_urls = process_text_and_extract_images(option)
        all_option_texts.append(option_text)
        downloaded_option_images = download_images(option_img_urls, f"option_{index}")
        all_option_images.append(downloaded_option_images)
    option_texts.append(','.join(all_option_texts))
    option_images.append(all_option_images)

# 确定最大图片数量，用于构建新的列（确保题目和选项的图片列对齐）
max_question_images = max(len(x) for x in question_images)
max_option_images = max(len(x) for x in option_images)
max_images = max(max_question_images, max_option_images)

# 创建新的DataFrame，添加处理后的列以及图片列
new_data = pd.DataFrame()
new_data['original_question'] = data['question']  # 保留原始题目列，可按需删除
new_data['processed_question'] = question_texts
for i in range(max_images):
    new_data[f'question_image_{i + 1}'] = [img_list[i] if i < len(img_list) else None for img_list in question_images]
new_data['original_options'] = data['options']  # 保留原始选项列，可按需删除
new_data['processed_options'] = option_texts
for i in range(max_images):
    new_data[f'option_image_{i + 1}'] = [img_list[i] if i < len(img_list) else None for img_list in option_images]

# 将结果保存到新的Excel文件
new_data.to_excel('processed_file.xlsx', index=False)
# 架构说明

## Magic.Cxxy.DbServices
额外的数据仓库，项目暂时用不到，忽略

## Magic.Guangdong.Assistant
常用的类库，基本都是单例模式，也有部分需要在请求周期内注入使用的模式

## Magic.Guangdong.DbServices
数据仓库，考试相关的Repo，接口，实体，dto等都在这里

## Magic.Guangdong.Exam.Client
考试客户端层

## Magic.Guangdong.Exam.Teacher
教师层

## Magic.Guangdong.Exam
管理层，除了后台web项目，一些计划任务等需要在后台静默执行的服务也在这里面

## Magic.PassportDbServices
用户中心的数据服务

## Magic.Service.Pay
支付服务，已经创建单独的微服务项目，后续将移除此项目

## k6脚本
压测的脚本

## mapster-tool
数据映射中间件
# 版本变更记录

### v1.0.2
2023.10.30
[new] 添加MPUIKet插件,这是个好用的程序化UI插件
[new] 添加一个场景管理组件SceneManagementComponent,用来处理Unity场景加载卸载时的需要执行的逻辑.
[new] 预添加资产管理组件AssetManagementComponent,功能未实现 
[new] 添加基于邻接表模式实现的层级树数据结构TreeDataModel,可用于后续的VFS可视化和ECS可视化等
[new] 添加两个实用类StringUtility和TimestampUtility,StringUtility.Compare用于对字符串进行字典序排序,TimestampUtility用于不同精度的时间戳转换
[new] 添加UrlUtility,手动实现encode和decode编码算法,不再依赖unity和dotnet的实现,删除原先IUrlHelper接口及其实现 
[new] 添加泛型TreeView,基于TreeModel重新实现unityGui的Treeview,用于后续在eidtor下绘制树结构的操作界面
[new] 添加VirtualFileSystem虚拟文件系统,并基于TreeView实现在编辑器中的Browser可视化交互界面
[new] 添加两个计算MD5相关的实用方法 

[fix] 修复服务端Entry编译报错的bug,Entry移除了OnStop抽象方法,Server的实现类没有同步移除导致编译错误. 
[fix] 修复ReservedAssembly编译错误,当工程目标框架是2.0时,部分4.x的API引用缺失导致编译失败,现在把整个脚本归入ENABLE_HYBRIDCLR宏内. 
[fix] 修正HybridCLR启发文档AOT列表为空时,AOT包构建失败的bug,现在遇到AOT清单是空的时候,打印一个提示并跳过AOT流程 

[update] 入口从实例首个Entry改为实例所有扫描到的Entry,由开发者自行决定启动哪个Entry,可以启动一个也可以启动多个 
[update] Entry入口类增加一个虚属性IsActivate,实现类可以重写属性的返回值,以决定该Entry的OnStart是否应该被执行. 
[update] 调整场景管理组件SceneManagementComponet,将执行启动场景OnLoaded的时机从Awake改为手动执行

### v1.0.1
2023.10.14
[fix] 修复GetComponentInChildren递归错误的bug,当递归第一个子实体返回null后,错误的提前结束递归.
[fix] 同步hybridclr的目标版本到4.0.10,修复ios端工具链编译报错的bug

### v1.0.0
2023.10.12

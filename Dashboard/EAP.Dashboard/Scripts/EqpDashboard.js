
layui.use(['carousel', 'form', 'slider', 'laydate', 'element','table'], function () {
    var carousel = layui.carousel
        , form = layui.form
        , laydate = layui.laydate
        , slider = layui.slider
        , element = layui.element
        , table = layui.table;
    var c_option = {
        elem: '#carousel'
        , width: '100%' //设置容器宽度
        , height: '95%'
        , arrow: 'always' //始终显示箭头
        , interval: 5000
        , indicator: 'outside'
        , autoplay: true // 是否自动播放
    };

    var ins = carousel.render(c_option);
    var statusFilter;
    var typeFilter;
    var allEqpData = []; // 保存全部设备
    var oeedata = [];    // 保存OEE数据

    // 初始化加载，调用一次
    function InitEqpCards(data, oeedataList) {
        allEqpData = data;
        oeedata = oeedataList;
        GenerateEqpCards(allEqpData, oeedata);
    }

    // 搜索按钮
    $('button[lay-filter="searchBtn"]').on('click', function () {
        var keyword = $('#eqidSearch').val().trim().toUpperCase();
        if (keyword === "") {
            layer.msg('请输入EQID关键词');
            return;
        }

        var filtered = allEqpData.filter(e => e.EQID.toUpperCase().includes(keyword));

        if (filtered.length === 0) {
            layer.msg('未找到匹配设备');
        }

        GenerateEqpCards(filtered, oeedata);
    });

    // 清除按钮
    $('button[lay-filter="clearBtn"]').on('click', function () {
        $('#eqidSearch').val('');
        GenerateEqpCards(allEqpData, oeedata);
    });

    // 外部调用入口
    window.InitEqpCards = InitEqpCards;

    $(function () {
        //resizeFontSize();
        getData();
        ChartssetInterval();
    })//设置时间初始值
    function ChartssetInterval() {
        //每1分钟刷新一次界面
        setInterval(function () {
            getData();
        }, 30000);
    }

    function keepSessionAlive() {
        fetch('EquipmentDashboard/RefreshSession', {
            method: 'GET',
            credentials: 'same-origin'
        }).then(response => {
            if (response.ok) {
                console.log('Session refreshed');
            } else {
                console.log('Failed to refresh session');
            }
        }).catch(error => {
            console.error('Error refreshing session:', error);
        });
    }
    // 每5分钟（300000毫秒）刷新一次会话
    setInterval(keepSessionAlive, 300000);

    window.ShowDetails = function (EQID) {
        console.log(EQID)
        const eqp = encodeURIComponent(EQID);

        var url = 'EquipmentDashboard/Details?eqp=' + eqp;

        layui.use([], function () {
            layer.open({
                title: EQID
                , type: 2
              /*  , btn: ['OK']*/
                , content: url //'/Dashboard/BoxDetails'
                , area: ['95%', '90%']
                , success: function (layero, index) {
                  
                }
                , yes: function (index) {
                    layer.close(index);
                }

            });

        });
    }
    
    function getData() {

        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "statusFilter": statusFilter,
                "typeFilter": typeFilter,
            },
            url: 'EquipmentDashboard/GetData',//
            success: function (retdata) {
                //console.log(retdata);
                var data = retdata.carddata;
                var oeedata = retdata.oeedata;
                InitEqpCards(data, oeedata)
                //GenerateEqpCards(data, oeedata);
                if (statusFilter == null && typeFilter == null) {

                    //GenerateSummaryPieChart(retdata.typedata)
                    //console.log(retdata.typedata)
                    const containerHeight = document.getElementById('eqp-type-container').clientHeight;
                    const containerWidth = document.getElementById('eqp-type-container').clientWidth;
                    if (retdata.typedata) {
                        table.render({
                            elem: '#eqp-type-table',
                            height: containerHeight,
                            layFilter: 'eqp-type-table',
                            width: containerWidth,
                            cols: [[
                                { field: 'name', title: 'EQP Type', align: "center", width: '60%', templet: '<div style="word-break: break-all;">{{d.name}}</div>' },
                                { field: 'value', title: 'EQP Count', align: "center", width: "40%", unresize: true },
                                
                            ]],
                            data: retdata.typedata,
                            size: 'sm',
                            page: false,
                            scroll: false,
                            loading: true,
                            limit: retdata.typedata.length,
                            done: function (res, curr, count) {
                                var tableElem = this.elem.next('.layui-table-view');

                                
                                // 禁止横向滚动条
                                tableElem.find('.layui-table-body').css({
                                    'overflow-x': 'hidden',
                                    'overflow-y': 'auto',
                                    'background': '#1E1E2F'
                                });

                                // 表体区域滚动
                                tableElem.find('.layui-table-body').css({
                                    'overflow-y': 'auto'
                                });

                                // 增加每行之间的间隙
                                tableElem.find('.layui-table-body tbody tr').css({
                                    'margin-bottom': '6px',
                                    'border-spacing': '0',
                                    'background': '#1E1E2F', // 深色背景更协调
                                    'cursor': 'pointer'  // 悬浮变手势
                                }).attr('title', '点击筛选该类型'); // 添加提示文本

                                // 给单元格加内边距
                                tableElem.find('.layui-table-cell').css({
                                    //'padding': '6px 4px',
                                    'white-space': 'normal', // 文字换行
                                    'word-break': 'break-word'
                                });
                                $('.layui-table-body tbody tr').on('click', function () {
                                    var index = $(this).index();
                                    var rowData = retdata.typedata[index];
                                    console.log('点击行数据:', rowData);
                                    typeFilter = rowData.name;
                                    getData()

                                });
                                // 可选：添加悬浮高亮效果
                                $('.layui-table-body tbody tr').hover(
                                    function () {
                                        $(this).css('background-color', '#2E2E45'); // 悬浮颜色
                                    },
                                    function () {
                                        $(this).css('background-color', '#1E1E2F'); // 恢复默认
                                    }
                                );

                            

                                
                            }
                        });


                     
                    }
                    GenerateSummaryPieChartStatus(retdata.statusdata)
                }

            },
            error: function () {
            }
        });

    }
 
    function GenerateEqpCards(data, oeedata) {
        var pagecount = Math.ceil(data.length / 12);
        var pageText = '';

        for (var page = 0; page < pagecount; page++) {
            pageText += '<div class="layui-row layui-row-amhs eqp-card-row row-cols-3" style="background: rgba(0,0,0,0);">';

            for (var cardnum = page * 12; cardnum < (page + 1) * 12 && cardnum < data.length; cardnum++) {
                var eqp = data[cardnum];

                var v = eqp.RunRate * 255;
                var runrate = (eqp.RunRate * 100).toFixed(2);
                var color = 'rgb(' + (255 - v) + ',' + v + ',0)';

                // 柔和颜色对应状态
                var sysStateColorMap = new Map([
                    ['Unknow', '#311B92'],    // 深紫
                    ['Idle', '#F9A825'],       // 柔黄
                    ['Run', '#1B5E20'],        // 柔绿
                    ['Alarm', '#B71C1C'],      // 柔红
                    ['Offline', '#424242'],    // 灰
                    ['Down', '#0D47A1']        // 浅青蓝
                ]);

                // 添加 OEE 数据
                for (var i = 0; i < oeedata.length; i++) {
                    var eqpoee = oeedata[i];
                    if (eqp.EQID === eqpoee.EQID) {
                        eqp.oee = eqp.EQID === 'CONTI_001'
                            ? (parseFloat(eqpoee.Value) / 4800).toFixed(2)
                            : eqpoee.Value;
                    }
                }

                var stateColor = sysStateColorMap.get(eqp.Status) || '#B0BEC5'; // 默认灰蓝

                pageText += `
                <div class="layui-col-md1 agv-card" onclick="ShowDetails('${eqp.EQID}')">
                    <div class="box-status">
                        <div class="layui-card-header eqp-name" style="background-color: ${stateColor}; color: white;">
                            ${eqp.Name}
                        </div>
                        <div class="layui-card-body eqp-details">
                            <table>
                                <tbody>
                                    <tr><th>Status</th><td>${eqp.Status}</td></tr>
                                    <tr><th>Type</th><td>${eqp.Type}</td></tr>
                                    <tr><th>MTBA</th><td>${(eqp.Mtba * 1).toFixed(2)}</td></tr>
                                </tbody>
                            </table>
                            <div class="runrate-container">
                                <div class="progress">
                                    <div class="progress-bar" style="width: ${runrate}%; background-color: ${color}">
                                        <span class="progress-text">Runrate: ${runrate}%</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`;
            }

            pageText += '</div>';
        }

        document.getElementById("carouselcontent").innerHTML = pageText;
        ins.reload(c_option);
    }



    function GenerateEqpCardsOld(data, oeedata) {
        var pagecount = data.length / 15;
        var pageText = '';
        for (var page = 0; page < pagecount; page++) {
            pageText += '<div class="layui-row layui-row-amhs" style="background: rgba(0,0,0,0); ">'

            for (var cardnum = page * 15; cardnum < (page + 1) * 15 && cardnum < data.length; cardnum++) {
                var eqp = data[cardnum];

                var v = eqp.RunRate * 255;
                var runrate = (eqp.RunRate * 100).toFixed(2);
                var color = 'rgb(' + (255 - v) + ',' + v + ',0)';
                var sysStateColorMap = new Map([['Unknow', 'layui-bg-cyan'], ['Idle', 'layui-bg-orange']
                    , ['Run', 'layui-bg-green'], ['Alarm', 'layui-bg-red'], ['Offline', 'layui-bg-black'], ['Down', 'layui-bg-black']
                ]);

                //Rainy Add OEE related.
                for (var i = 0; i < oeedata.length; i++) {
                    var eqpoee = oeedata[i];

                    if (eqp.EQID == eqpoee.EQID && eqp.EQID == 'CONTI_001') {
                        var oee = (parseFloat(eqpoee.Value) / 4800).toFixed(2);
                        eqp.oee = oee;
                    }
                    else if (eqp.EQID == eqpoee.EQID) {
                        var oee = eqpoee.Value;
                        eqp.oee = oee;
                    }

                }

                //console.log(oee);
                var stateColor = sysStateColorMap.get(eqp.Status);

                pageText += '<div class="layui-col-md1 agv-card" onclick="ShowDetails(\'' + eqp.EQID + '\')">'
                    + '<div class="box-status" style="height: 100%;  ">'
                    + '<div class="layui-card-header ' + stateColor + '" style="font-size: 0.2rem; height: 23%">'
                    + eqp.Name
                    + '</div>'
                    + '<div class="layui-card-body" style="font-size: 0.15rem;opacity:1; height: 75% ">'
                    + '<table><tbody>'
                    + '<tr><th>Status</th><td>' + eqp.Status + '</td></tr>'
                    + '<tr><th>Type</th><td>' + eqp.Type + '</td></tr>'
                    + '<tr><th>MTBA</th><td>' + (eqp.Mtba * 1).toFixed(2) + '</td></tr>'
                    //+ '<tr><th>Run Rate</th><td>' + runrate +'</td></tr>'
                    + '</tbody></table>'
                //+ '<div style="height: 15%">Status: ' + eqp.Status + '</div>';
                //if (eqp.PD != undefined) {
                //    pageText += '<div style="height: 15%">Location: ' + eqp.PD + '</div>';
                //}
                //    //if(eqp.LINE != undefined) pageText += '<div style="height: 15%">Location: ' + eqp.PD + eqp.LINE + '</div>';
                //else {
                //    pageText += '<div style="height: 15%">Location: ' + '</div>';
                //}
                //+ '<div style="height: 15%">Location: ' + eqp.PD + eqp.LINE + '</div>'
                //+ '<div style="height: 15%">OEE: ' + eqp.oee + '</div>'
                pageText //+= '<div style="height: 18%"><i class="layui-icon"></i>Type：' + eqp.Type + '</div>'
                    //+ '<div style="height: 15%"><i class="layui-icon"></i>MTBA: ' + (eqp.Mtba * 1).toFixed(2) + '</div>'
                    //+ '<div style="height: 15%"><i class="layui-icon"></i>Runrate: ' + runrate + '</div>'
                    //+ '<div style="position: relative;  width: 100%; height: 17% ">'
                    += '<div>Runrate</div><div class="progress">'
                    + '<div class="progress-bar" style="width: ' + runrate + '%; background-color: ' + color + '">' + runrate + '%</div>'
                    + '</div></div></div></div>';
            }

            pageText += ' </div>';
        }

        document.getElementById("carouselcontent").innerHTML = pageText;
        ins.reload(c_option);

    }


    function GenerateSummaryPieChart(typedata) {
        //console.log(statusdata);
        var myChart = echarts.init(document.getElementById('summaryPieChart'));
        var xArr = [];
        var yArr = [];


        var option = {
            title: {
                top: '3%',
                text: 'Equipment Type',
                left: 'center',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }
            },
            tooltip: {
                trigger: 'item',
                formatter: '{a} <br/>{b}: {c} ({d}%)'
            },
            series: [

                {
                    name: 'Equipment Type',
                    type: 'pie',
                    radius: ['10%', '40%'],
                    label: {
                        color: 'white',
                    },
                    labelLine: {
                        length: 3,
                        normal: {
                            lineStyle: {
                                color: 'rgba(255, 255, 255, 0.3)'
                            },
                            smooth: 0.2,
                            size: 30
                            //length: this.standSize / 50,
                            //length2: this.standSize / 100,
                        }
                    },

                    data: typedata
                }
            ]
        };
        /*   */
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
        window.addEventListener("resize", function () {
            myChart.resize();
        });
        myChart.on('click', function (param) {
            var status = ['Run', 'Alarm', 'Idle', 'Unknow', 'Offline', 'Down']
            var a = status.indexOf(param.name);
            if (a > -1) {
                statusFilter = param.name;
            }
            else {
                typeFilter = param.name;
            }


            getData();


        })
    }

    function GenerateSummaryPieChartStatus(statusdata) {
        var myChart = echarts.init(document.getElementById('summaryPieChart-status'));

        // 柔和颜色映射
        const statusColorMap = {
            'Alarm': '#E57373',    // 柔红
            'Run': '#81C784',      // 柔绿
            'Idle': '#FFF176',     // 柔黄
            'Down': '#0D47A1',     // 深蓝灰
            'Offline': '#757575',  // 灰色
            'Unknown': '#BA68C8'   // 柔紫
        };

        // 为每个状态设置颜色
        const coloredData = statusdata.map(item => ({
            ...item,
            itemStyle: {
                color: statusColorMap[item.name] || '#B0BEC5' // 默认灰蓝色
            }
        }));

        const option = {
            title: {
                top: '3%',
                text: 'Equipment Status',
                left: 'center',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }
            },
            //legend: {
            //    top: 'bottom',
            //    left: 'left',
            //    orient: 'vertical',
            //    textStyle: {
            //        color: 'white',
            //        fontSize: 12
            //    },
            //    formatter: (name) => {
            //        let total = 0;
            //        let tarValue;
            //        for (let i = 0; i < statusdata.length; i++) {
            //            total += statusdata[i].value;
            //            if (statusdata[i].name === name) {
            //                tarValue = statusdata[i].value;
            //            }
            //        }
            //        return `${name}: ${tarValue}`;
            //    }
            //},
            grid: {
                top: '13%'
            },
            tooltip: {
                trigger: 'item',
                formatter: '{a} <br/>{b}: {c} ({d}%)'
            },
            series: [
                {
                    name: 'Equipment Status',
                    type: 'pie',
                    selectedMode: 'single',
                    radius: [0, '46%'],
                    label: {
                        color: 'white',
                        formatter: (param) => {
                            let name = param.name
                            let total = 0;
                            let tarValue;
                            for (let i = 0; i < statusdata.length; i++) {
                                total += statusdata[i].value;
                                if (statusdata[i].name === name) {
                                    tarValue = statusdata[i].value;
                                }
                            }
                            return `${name}: ${tarValue}`;
                        }
                    },
                    labelLine: {
                        normal: {
                            lineStyle: {
                                color: 'rgba(255, 255, 255, 0.3)'
                            },
                            smooth: 0.2,
                            length: 3,
                        }
                    },
                    data: coloredData
                }
            ]
        };

        myChart.setOption(option);

        window.addEventListener("resize", function () {
            myChart.resize();
        });

        myChart.on('click', function (param) {
            const statusList = ['Run', 'Alarm', 'Idle', 'Unknow', 'Offline', 'Down'];
            if (statusList.includes(param.name)) {
                statusFilter = param.name;
            } else {
                typeFilter = param.name;
            }
            getData();
        });
    }


    function GenerateSummaryPieChartStatus2(statusdata) {
        //console.log(statusdata);
        var myChart = echarts.init(document.getElementById('summaryPieChart-status'));
        var xArr = [];
        var yArr = [];


        var option = {
            title: {
                top: '3%',
                text: 'Equipment Status',
                left: 'center',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }
            },

            legend: {
                top: 'bottom',
                left: 'left',
                orient: 'vertical',
                textStyle: {
                    color: 'white',
                    fontSize: 12
                },
                formatter: (name) => {
                    let total = 0;
                    let tarValue;
                    for (let i = 0; i < statusdata.length; i++) {
                        total += statusdata[i].value;
                        if (statusdata[i].name == name) {
                            tarValue = statusdata[i].value;
                        }
                    }
                    return `${name}: ${tarValue}`;
                },
            },
            grid: {
                /*left: '15%',*/
                top: '13%'
            },
            tooltip: {
                trigger: 'item',
                formatter: '{a} <br/>{b}: {c} ({d}%)'
            },
            series: [
                {
                    name: 'Equipment Status',
                    type: 'pie',
                    selectedMode: 'single',
                    radius: [0, '46%'],
                    label: {
                        color: 'white',
                    },
                    labelLine: {
                        length: 3,
                        normal: {
                            lineStyle: {
                                color: 'rgba(255, 255, 255, 0.3)'
                            },
                            smooth: 0.2,
                            size: 30
                            //length: this.standSize / 50,
                            //length2: this.standSize / 100,
                        }
                    },
                    data: statusdata
                }
            ]
        };
        /*   */
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
        window.addEventListener("resize", function () {
            myChart.resize();
        });
        myChart.on('click', function (param) {
            var status = ['Run', 'Alarm', 'Idle', 'Offline', 'Down','Unknow']
            var a = status.indexOf(param.name);
            if (a > -1) {
                statusFilter = param.name;
            }
            else {
                typeFilter = param.name;
            }


            getData();


        })
    }


    var allEqpData = [];  // 存储原始数据

    function InitEqpCards(data, oeedata) {
        allEqpData = data;  // 备份
        GenerateEqpCards(data, oeedata);
    }

   
    
});







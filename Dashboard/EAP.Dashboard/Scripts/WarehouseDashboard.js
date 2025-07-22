layui.use(['table', 'jquery', 'form', 'laydate'], function () {
    var table = layui.table
        , form = layui.form
        , $ = layui.jquery
        , laydate = layui.laydate;

    form.render();
    form.render("select");
    var eqpdata = [];
    var eqid;
    var today = new Date();
    var datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
    $("#datepicker").val(datetime);

    setInterval(DisplayHistoryData, 300 * 60 * 1000);//300分钟执行一次，5h
    setInterval(DisplayRealtimeData, 10 * 60 * 1000);//10分钟执行一次

    DisplayHistoryData();
    DisplayRealtimeData();
    function DisplayHistoryData() {
        console.log("refresh history data!")
        datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
        $("#datepicker").val(datetime);
        GetHistoryData();
        GetAGVHistoryData();

    }

    function DisplayRealtimeData() {
        console.log("refresh dashboard!")
        GetRealtimeData();
        GetAVGSuceessRate();
        GetCacheWallStorageA();
    }



    laydate.render({
        elem: '#datepicker'

        //, value: date
        , done: function (value, date, endDate) {
            console.log(value)
            datetime = value;
            GetHistoryData();
            GetAGVHistoryData();

        }

    });

    $("#download-report").click(function () {
        layer.open({
            title: '下载报表'
            , type: 2
            , btn: ['确定']
            , content: 'RIDM/DownloadReports'
            , area: ['80%', '90%']
            , success: function (layero, index) {
                ////向layer页面传值，传值主要代码
                var body = layer.getChildFrame('body', index);
                //body.find("[id='equipment']").val(e.data.equipment);
                //body.find("[id='station']").val('Station_' + (e.data.value[0] + 1));
                body.find("[id='dateFilter']").val(datetime);
            }
            , yes: function (index) {
                layer.close(index);
            }

        });
    })

    /*获取机种、料号、工单号、设备利用率、合盘数*/
    function GetRealtimeData() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            url: 'WarehouseDashboard/GetRealtimeData',
            success: function (retdata) {
                //var params = retdata.paramList;
                console.log(retdata)
                var achieveRate_7 = retdata.achieveRatio * 100
                var cacheWallCnt = retdata.cacheWallCnt
                var totalCnt = retdata.totalCnt
                var unloadCnt = retdata.unloadCnt
                var cacheWall_order_ratio = (cacheWallCnt / totalCnt) * 100
                var cacheWall_order = [{ name: "缓存墙出库", value: cacheWallCnt }, { name: "Unload发料", value: unloadCnt }]

                drawGuage('achieve_rate', achieveRate_7.toFixed(1), 100, '7寸盘发料达成率')
                drawGuage('unload_rate_1', 100, 100, '1# Unload \n\n 7寸发料成功率')
                drawGuage('unload_rate_2', 100, 100, '2# Unload \n\n 7寸发料成功率')

                drawOutputPie('cache_wall_pie', cacheWall_order, "WMS发料总盘数: " + totalCnt + "\n\n缓存墙工单任务占比: " + cacheWall_order_ratio.toFixed(2) + '%')


            },
            error: function () {

            }
        });
    }

    function GetAVGSuceessRate() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            url: 'WarehouseDashboard/GetAVGSuceessRate',
            success: function (retdata) {
                console.log(retdata)
                const containerHeight = document.getElementById('table-data').clientHeight;
                const containerWidth = document.getElementById('table-data').clientWidth;
                layui.table.render({
                    elem: '#table-data-ctu'
                    , height: containerHeight
                    , width: containerWidth   // 固定表格宽度
                    , cols: [[ //标题栏
                        /* { field: 'AlarmEqp', title: 'EQID', align: "center", width: '20%' }*/
                        { field: 'CTU_ID', title: 'CTU ID', align: "center", width: "20%", unresize: true }
                        /*, { field: 'CA', title: 'Carrier ID', align: "center", width: '15%' }*/
                        , { field: 'TotalTasks', title: '总任务数', align: "center", width: "20%", unresize: true }
                        , { field: 'Status4Count', title: '完成数', align: "center", width: "20%", unresize: true }
                        , { field: 'Status6Count', title: '异常数', align: "center", width: "20%", unresize: true }
                        , {
                            field: 'Ratio', title: '成功率', align: "center", width: "20%", unresize: true
                        }

                    ]]
                    , data: retdata
                    , size: 'md'
                    , page: false //开启分页
                    , scroll: true // 开启横向滚动
                    , loading: true
                    , limit: retdata.length
                    , done: function (res, curr, count) {

                        //$(".layui-table tr").css("height", "20%");
                    }


                });
            },
            error: function () {

            }
        });
    }

    function GetCacheWallStorageA() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            url: 'WarehouseDashboard/GetCacheWallStorageA',
            success: function (retdata) {
                console.log(retdata)
                //drawHeatmap("cache_wall_csv",retdata,"A区空箱位数")

                var totalStock = retdata.totalStock;
                var stockNotEmpty = retdata.stockNotEmpty;
                var stockEmpty = retdata.stockEmpty;
                let externalData = retdata.result.map(item => {
                    return { name: item.ZONE, value: item.NotEmptyPositions }
                });
                let innerData = [{ name: "有料数", value: stockNotEmpty }, {
                    name: "空箱数", value: stockEmpty.length
                }]
                console.log(innerData)
                drawTotalOutputPie("storage-pie", externalData, innerData, "A区/B区库位情况");
                document.getElementById("total-stock").innerHTML = totalStock;
                document.getElementById("not-empty-stock").innerHTML = stockNotEmpty;
                document.getElementById("empty-stock").innerHTML = stockEmpty.length;

                const containerHeight = document.getElementById('empty-position').clientHeight;
                const containerWidth = document.getElementById('empty-position').clientWidth;

                layui.table.render({
                    elem: '#table-empty-position'
                    , height: containerHeight
                    , width: containerWidth   // 固定表格宽度
                    , cols: [[ //标题栏
                        /* { field: 'AlarmEqp', title: 'EQID', align: "center", width: '20%' }*/
                        /*  { field: 'UPDATE_TIME', title: '更新时间', templet: '<div>{{ FormDate(d.UPDATE_TIME, "MM-dd HH:mm:ss") }}</div>', align: "center", width: 0, unresize: true }*/
                        /*, { field: 'CA', title: 'Carrier ID', align: "center", width: '15%' }*/
                        { field: 'LOCATION_ZONE', title: '区域', align: "center", width: '20%', unresize: true }
                        , { field: 'LOCATION_CODE', title: '空箱储位号', align: "center", width: '40%', unresize: true }
                        , { field: 'TRAY_CODE', title: '空箱号', align: "center", width: '40%', unresize: true }

                    ]]
                    , data: stockEmpty
                    , size: 'sm'
                    , page: false //开启分页
                    , scroll: true // 开启横向滚动
                    , loading: true
                    , limit: stockEmpty.length
                    , done: function (res, curr, count) {


                    }


                });
            },
            error: function () {

            }
        });
    }
    function GetHistoryData() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "datetime": datetime,
            },
            url: 'WarehouseDashboard/GetHistoryData',
            success: function (retdata) {
                console.log(retdata)
                const dates = retdata.map(item => item.Date);
                const rateAList = retdata.map(item => item.Rate * 100);
                const rateBList = retdata.map(item => item.Unload1 * 100);
                const rateCList = retdata.map(item => item.Unload2 * 100);
                drawhistoryRate("trend-rate-1", rateAList, dates, "7寸盘发料达成率趋势")
                drawhistoryRate("trend-unload-1", rateBList, dates, "1# Unload 7寸发料成功率趋势")
                drawhistoryRate("trend-unload-2", rateCList, dates, "2# Unload 7寸发料成功率趋势")

                //drawGuage('guage-data-utilization-' + eqpNumber, utilizationArr[utilizationArr.length - 1], 100, eqpNumber + '号机-设备利用率(%)')
            },
            error: function () {

            }
        });
    }
    function GetAGVHistoryData() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "datetime": datetime,
            },
            url: 'WarehouseDashboard/GetAGVHistoryData',
            success: function (retdata) {
                console.log(retdata)
                const dates = retdata.map(item => item.Date);
                drawhistoryAgvTasks("agv-task-bar", retdata, "CTU搬运任务成功数")
                //drawGuage('guage-data-utilization-' + eqpNumber, utilizationArr[utilizationArr.length - 1], 100, eqpNumber + '号机-设备利用率(%)')
            },
            error: function () {

            }
        });
    }

    function drawHeatmap(id, data, name) {
        var heatmapData = data.map(item => [item.X, item.Y, item.Value]);
        //data.map(function (item) {
        //    return [item[0], item[1], item[2]]
        //});
        var codeMap = data.reduce((map, item) => {
            map[`${item.X},${item.Y}`] = item.Base;
            return map;
        }, {});
        console.log(codeMap);
        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;

        var myChart = echarts.init(chartDom, 'dark');
        var option;

        option = {
            title: {
                text: name,
                top: '5%',
                left: 'left',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }


            },
            tooltip: {
                position: 'top',
                formatter: function (params) {
                    //var codes = params.data[3].join("<br>"); // 取出 Codes 数组
                    return `位置: (${params.data[0]}, ${params.data[1]})<br>
                                    库存为0的库位数: ${params.data[2]}<br>`;
                    //具体位置:<br>${codes}`;
                }
            },
            grid: {
                //height: '80%',
                //top: '5%'
                left: '0%',
                right: '3%',
                top: '3%',
                bottom: '0%',
                containLabel: true // 确保图形不被边界裁剪
            },
            xAxis: { type: 'category', show: false },
            yAxis: { type: 'category', show: false },
            visualMap: {
                show: false,
                min: 0,
                max: 10,
                calculable: true,
                orient: 'horizontal',
                left: 'left',
                top: '10%'
            },
            series: [{
                type: 'heatmap',
                data: heatmapData,
                label: {
                    show: true,
                    formatter: function (params) {
                        var key = `${params.value[0]},${params.value[1]}`;
                        var result = "";
                        if (params.value[2] > 0) {
                            result = codeMap[key];
                        }
                        return result;
                        //return params.value[3]; // 显示 code
                    },
                },
                emphasis: { itemStyle: { borderColor: '#333', borderWidth: 1 } }
            }]
        };

        option && myChart.setOption(option);
    }
    function drawGuage(id, data, maxdata, name) {

        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;

        var myChart = echarts.init(chartDom, 'dark');
        var option;
        option = {
            title: {
                text: name,
                bottom: '10%',
                left: 'center',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }


            },
            series: [
                {
                    type: 'gauge',
                    radius: '55%',
                    center: ['50%', '55%'],
                    startAngle: 200,
                    endAngle: -20,
                    min: 0,
                    max: maxdata,
                    splitNumber: 10,
                    itemStyle: {
                        color: '#FFAB91'
                    },
                    progress: {
                        show: true,
                        width: 30
                    },
                    pointer: {
                        show: false
                    },
                    axisLine: {
                        lineStyle: {
                            width: 30
                        }
                    },
                    axisTick: {
                        distance: -45,
                        splitNumber: 5,
                        lineStyle: {
                            width: 2,
                            color: '#999'
                        }
                    },
                    splitLine: {
                        distance: -52,
                        length: 14,
                        lineStyle: {
                            width: 3,
                            color: '#999'
                        }
                    },
                    axisLabel: {
                        distance: -10,
                        color: '#999',
                        fontSize: 14
                    },
                    anchor: {
                        show: false
                    },
                    title: {
                        show: false

                    },
                    detail: {
                        valueAnimation: true,
                        width: '55%',
                        lineHeight: 40,
                        borderRadius: 12,
                        offsetCenter: [0, '-15%'],
                        fontSize: 20,
                        fontWeight: 'bolder',
                        formatter: '{value}%',
                        color: 'inherit'
                    },
                    data: [
                        {
                            value: data
                        }
                    ]
                },
                {
                    type: 'gauge',
                    radius: '55%',
                    center: ['50%', '55%'],
                    startAngle: 200,
                    endAngle: -20,
                    min: 0,
                    max: maxdata,
                    itemStyle: {
                        color: '#FD7347'
                    },
                    progress: {
                        show: true,
                        width: 12
                    },
                    pointer: {
                        show: false
                    },
                    axisLine: {
                        show: false
                    },
                    axisTick: {
                        show: false
                    },
                    splitLine: {
                        show: false
                    },
                    axisLabel: {
                        show: false
                    },
                    detail: {
                        show: false
                    },
                    data: [
                        {
                            value: data
                        }
                    ]
                }
            ]
        };
        option && myChart.setOption(option);
        myChart.resize();

    }

    function drawOutputPie(id, data, name) {
        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;

        var myChart = echarts.init(chartDom, 'dark');
        var option;


        option = {
            title: {
                //top: '2%',
                text: name,
                left: 'left',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }
            },
            legend: {
                bottom: '3%',
                left: 'left',
                orient: 'vertical',
                textStyle: {
                    color: 'white',
                    fontSize: 13
                },
                formatter: (name) => {
                    let total = 0;
                    let tarValue;
                    for (let i = 0; i < data.length; i++) {
                        total += data[i].value;
                        if (data[i].name == name) {
                            tarValue = data[i].value;
                        }
                    }
                    return `${name}  ${tarValue} 盘`;
                },

            },
            tooltip: {
                trigger: 'item',
                extraCssText: 'width:auto;height:auto'
            },
            series: [

                {

                    type: 'pie',
                    radius: ['10%', '35%'],
                    avoidLabelOverlap: false,
                    padAngle: 5,
                    itemStyle: {
                        borderRadius: 10
                    },
                    //itemStyle: {
                    //    borderColor: '#fff',
                    //    borderWidth: 1
                    //},
                    label: {
                        //alignTo: 'edge',
                        //formatter: '{name|{b}}\n\n{output|{c} 盘}',
                        formatter: function (params) {
                            var value = parseFloat(params.value).toFixed(0)
                            if (value > 0) {
                                return `{name|${params.name}}\n\n{output|${params.value} 盘}`;
                            } else {
                                return '';
                            }


                        },

                        minMargin: 20,
                        //edgeDistance: '7%',
                        lineHeight: 15,
                        rich: {
                            name: {
                                fontSize: 14,
                                fontWeight: 'bold',

                            },
                            output: {
                                fontSize: 13,
                            }
                        }
                    },
                    labelLine: {
                        length: 25, // 调整引导线的长度
                        smooth: true, // 使用平滑曲线
                    },
                    emphasis: {
                        label: {
                            show: true,
                            fontSize: 40,
                            fontWeight: 'bold'
                        }
                    },
                    //labelLine: {
                    //    length: 15,
                    //    length2: 0,
                    //    maxSurfaceAngle: 80
                    //},
                    labelLayout: function (params) {
                        const isLeft = params.labelRect.x < myChart.getWidth() / 2;
                        const points = params.labelLinePoints;
                        // Update the end point.
                        points[2][0] = isLeft
                            ? params.labelRect.x
                            : params.labelRect.x + params.labelRect.width;
                        return {
                            labelLinePoints: points
                        };
                    },

                    data: data,
                }

            ]
        };
        option && myChart.setOption(option);
        myChart.resize();

    }

    function drawTotalOutputPie(id, dataExternal, dataInner, name) {
        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;

        var myChart = echarts.init(chartDom, 'dark');
        var option;

        //var total = 0;
        //dataExternal.forEach(item => {
        //    total += item.value
        //});
        //console.log(total);
        option = {
            title: {
                top: '3%',
                text: name,
                left: 'left',
                textStyle: {
                    color: 'white',
                    fontSize: 20
                }
            },
            tooltip: {
                trigger: 'item',
                formatter: '{a} <br/>{b}: {c} ({d}%)',
                extraCssText: 'width:auto;height:auto'
            },
            toolbox: {
                show: true,
                bottom: 'bottom',
                feature: {
                    mark: { show: true },
                    dataView: { show: true, readOnly: false },
                    restore: { show: true },
                    saveAsImage: { show: true }
                }
            },
            //legend: {
            //    top: '3%',
            //    left: 'right'
            //},
            //legend: {
            //    data: dataExternal
            //},
            series: [

                {
                    name: '库位分布',
                    type: 'pie',
                    selectedMode: 'single',
                    radius: [0, '40%'],
                    //itemStyle: {
                    //    borderColor: '#fff',
                    //    borderWidth: 1
                    //},
                    label: {
                        position: 'inner',
                        formatter: function (param) {

                            //console.log(parseInt(param.name.split('S')[1]))
                            return `${param.name}:${param.value}`;
                        },
                        //formatter: '{b}:{c}盘',

                        fontSize: 14,
                        color: '#4C5058',
                        fontWeight: 'bold',
                        fontSize: 14,

                        textStyle: {
                            textShadowColor: 'rgba(255, 255, 255, 0.5)', // 阴影颜色

                        },
                    },
                    labelLine: {
                        show: false
                    },


                    data: dataInner,
                },
                {
                    name: '有料库位数',
                    type: 'pie',

                    radius: ['50%', '70%'],
                    //itemStyle: {
                    //    borderColor: '#fff',
                    //    borderWidth: 1
                    //},
                    label: {
                        alignTo: 'edge',
                        formatter: function (param) {
                            //var name = parseInt(param.name.split('S')[1]);
                            //console.log(parseInt(param.name.split('S')[1]))
                            return `${param.name}区有料库位数\n\n${param.value}`;
                        },

                        minMargin: 20,
                        edgeDistance: '2%',
                        lineHeight: 15,
                        fontSize: 17, // 字体大小
                        fontFamily: 'Arial', // 字体
                        fontWeight: 'bold', // 字体粗细

                        //rich: {
                        //    id: {
                        //       /* color: '#4C5058',*/
                        //        fontSize: 14,
                        //        fontWeight: 'bold',
                        //        //formatter: function (value) {
                        //        //    console.log(value)
                        //        //    return value.split('S').map(Number)+'号机'
                        //        //}
                        //      /*  lineHeight: 33*/

                        //    },
                        //    output: {
                        //        fontSize: 20,
                        //    }
                        //}
                    },
                    labelLine: {
                        length: 15,
                        length2: 0,
                        maxSurfaceAngle: 80
                    },
                    labelLayout: function (params) {
                        const isLeft = params.labelRect.x < myChart.getWidth() / 2;
                        const points = params.labelLinePoints;
                        // Update the end point.
                        points[2][0] = isLeft
                            ? params.labelRect.x
                            : params.labelRect.x + params.labelRect.width;
                        return {
                            labelLinePoints: points
                        };
                    },

                    data: dataExternal,
                }

            ]
        };
        option && myChart.setOption(option);
        myChart.resize();

    }
    function drawhistoryRate(id, rateData, dateData, name) {
        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;

        var myChart = echarts.init(chartDom, 'dark');
        var option;

        option = {
            title: {
                top: '3%',
                text: name,
                left: 'left',
                textStyle: {
                    color: 'white',
                    fontSize: 16
                }
            },
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'cross'
                },
                extraCssText: 'width:auto;height:auto'
            },
            axisPointer: {
                link: { xAxisIndex: 'all' },
                label: {
                    backgroundColor: '#777'
                }
            },
            legend: {
                left: 'right',
                top: '3%',
                textStyle: {
                    color: "#fff"
                }
            },
            grid: {
                left: '15%',
                top: '30%'
            },
            xAxis: {
                type: 'category',
                axisTick: {
                    alignWithLabel: true
                },
                data: dateData,
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    }
                },
                axisLabel: {
                    rotate: 25,
                    //fontSize: 13,
                }
            },
            yAxis: [
                {
                    type: 'value',
                    name: 'Rate',
                    position: 'left',
                    min: 0,
                    max: function (value) { return Math.ceil(value.max * 1.1) },
                    splitLine: {     //网格线
                        show: false,
                        lineStyle: {
                            color: '#22376d'
                        }
                    },
                    axisLine: {
                        lineStyle: {
                            color: "#fff",
                        }
                    },
                    axisLabel: {
                        formatter: '{value} %',
                        fontSize: 13,
                    }
                }],
            series: [
                {
                    name: 'Rate',
                    type: 'line',
                    data: rateData,
                    areaStyle: {
                        color: '#339933',
                    },
                    smooth: true,
                    label: {
                        show: true,
                        rotate: 25,
                        fontSize: 13,
                        position: 'top',
                        align: 'left',
                        color: 'white',
                        formatter: function (params) {
                            return parseFloat(params.value).toFixed(1) + '%'
                        }
                    },
                    lineStyle: {
                        color: 'yellow'
                    },
                    //markLine: {
                    //    silent: true,
                    //    itemStyle: {
                    //        normal: {
                    //            color: '#FA8565',
                    //        }
                    //    },
                    //    data: [{
                    //        yAxis: target,
                    //        label: {
                    //            color: 'white',
                    //        }
                    //    }]
                    //}
                }
            ]
        };
        option && myChart.setOption(option);
        myChart.resize();


    }
    function drawhistoryAgvTasks(id, data, name) {
        //var dates = [...new Set(data.map(item => item.name))];

        let groupedByDate = data.reduce((acc, item) => {
            if (!acc[item.Date]) {
                acc[item.Date] = {}
            }
            if (!acc[item.Date][item.CTU_ID]) {
                acc[item.Date][item.CTU_ID] = 0;

            }
            acc[item.Date][item.CTU_ID] = item.Status4Count;
            return acc;
        }, {})
        var dates = Object.keys(groupedByDate)
        let ctus_ids = Array.from(new Set(data.map(item => item.CTU_ID)));

        let series = ctus_ids.map(ctu_id => ({
            name: ctu_id,
            type: 'bar',
            data: dates.map(date => groupedByDate[date][ctu_id] || 0)
        }));

        console.log(groupedByDate)
        console.log(series)
        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;

        var myChart = echarts.init(chartDom, 'dark');
        var option;
        option = {
            title: {
                top: '3%',
                text: name,
                left: 'left',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }
            },

            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'shadow'
                },
                extraCssText: 'width:auto;height:auto'
            },
            axisPointer: {
                link: { xAxisIndex: 'all' },
                label: {
                    backgroundColor: '#777'
                }
            },
            legend: {
                data: ctus_ids,
                left: 'right',
                top: '3%',
                textStyle: {
                    color: "#fff"
                }
            },
            grid: {


                //height: '60%'
            },
            xAxis: {
                type: 'category',
                axisTick: {
                    alignWithLabel: true
                },
                data: dates,
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    }
                },
                axisLabel: {
                    rotate: 25
                }
            },
            yAxis: [
                {
                    type: 'value',
                    name: '任务数',
                    position: 'left',

                    min: 0,
                    max: function (value) { return Math.ceil(value.max) },
                    splitLine: {     //网格线
                        show: false,
                        lineStyle: {
                            color: '#22376d'
                        }
                    },
                    axisLine: {
                        lineStyle: {
                            color: "#fff",
                        }
                    },
                    axisLabel: {
                        formatter: '{value}'
                    }
                }],
            series: series
        };
        option && myChart.setOption(option);
        myChart.resize();
        myChart.on('click', function (param) {
            //console.log(param.name);


            //getStatusDetails(week);
        })


    }


})
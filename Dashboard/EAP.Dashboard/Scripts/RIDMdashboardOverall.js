layui.use(['table', 'jquery', 'form', 'laydate'], function () {
    var table = layui.table
        , form = layui.form
        , $ = layui.jquery
        , laydate = layui.laydate;

    form.render();
    form.render("select");
    var eqpdata = [];
    var eqid;

    setInterval(DisplayData, 1000000);

    var today = new Date();
    var datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
    $("#datepicker").val(datetime);
    // 获取所有 class 为 'myDiv' 的元素
    var divs = document.querySelectorAll(".eqpInfo");

    // 为每个 div 添加点击事件
    divs.forEach(function (div) {
        div.onclick = function () {
            var id = div.getAttribute("value");  // 读取当前 div 的 value 作为 ID
            console.log(id)
            var url = "RIDM/Detail?EQID=" + id;  // 将 ID 作为查询参数附加到 URL 上
            window.open(url, "_blank");  // 在新标签页打开目标页面
        };
    });
   

    laydate.render({
        elem: '#datepicker'

        //, value: date
        , done: function (value, date, endDate) {
            console.log(value)
            datetime = value;
            DisplayData();
            
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

    GetEQPList();
    
    // 定义一个字典对象
    const dict = {};
    addMapping("4to1", "4 to 1")
    addMapping("3to1", "3 to 1")
    addMapping("2to1", "2 to 1")

    // 添加映射关系
    function addMapping(key, value) {

        dict[key] = value;
    }


    // 获取映射结果
    function getMapping(key) {
        return dict[key] || '';
    }

    function GetEQPList() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                //"workshopFilter": $("#workshopFilter").val(),
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'RIDM/GetEQPList',
            success: function (retdata) {

                //$("#eqpFilter").html(retdata.option);
                eqpdata = retdata.eqList;
               
                DisplayData();
                //DisplayData();
            },
            error: function () {

            }
        });
    }

    function DisplayData() {
        //console.log(eqpdata)
        today = new Date();
        datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
        $("#datepicker").val(datetime);
        console.log('refresh dashboard:' + datetime)
        eqpdata.forEach(eqp => {
            //console.log(eqp)
            //GetEQPRealtimeData(eqp);
            GetEQPHistoryData(eqp);
        })
        GetTotalOutput();

    }


    function GetData() {
        console.log("Refresh Dashboard:" + eqid);
        //GetEQPRealtimeData();
        //GetEQPHistoryData();
        //GetHistoryUtilization();
        //GetHistoryOutput();
    }

    /*获取机种、料号、工单号、设备利用率、合盘数*/
    function GetEQPRealtimeData(eqp) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqp,//$("#workshopFilter").val(),
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'GetEQPRealtimeData',
            success: function (retdata) {
                var params = retdata.paramList;
                console.log(params)
                var utilization = 0;
                var outputs = [];
                params.forEach(item => {
                    switch (item.Name) {
                        case "utilization":
                            console.log("utilization: " + item.Value);
                            utilization = item.Value * 100;
                            break;
                        case "4to1":
                        case "3to1":
                        case "2to1":
                            console.log(item.Name + ": " + item.Value);
                            // 检查是否是当天的数据
                            const today = new Date();
                            const todayStr = today.toLocaleDateString();
                            const itemTimeStr = item.UpdateTime;
                            const itemTimestamp = parseInt(itemTimeStr.match(/\/Date\((\d+)\)\//)[1], 10);
                            const itemDate = new Date(itemTimestamp);
                            const itemDateStr = itemDate.toLocaleDateString();
                          
                            // 是当天的数据，累加到变量中
                            if (todayStr == itemDateStr) {
                                var outputItem = { name: getMapping(item.Name), value: item.Value };
                                outputs.push(outputItem);
                            } else {
                                // 不是当天的变量，该Value设置为 0
                                var outputItem = { name: getMapping(item.Name), value: '0' };
                                outputs.push(outputItem);
                            }
                            
                           
                            break;
                        default:
                            var targetID = item.Name + '-' + parseInt(eqp.split('S')[1]);
                            console.log(targetID)
                            document.getElementById(item.Name + '-' + parseInt(eqp.split('S')[1])).innerHTML = item.Value;
                            break;
                    }
                   
                })
                //console.log(outputs)
                var outputCounts = 0;
                outputs.forEach(item => {
                    //console.log(parseInt(item.value))
                    var eachVal = parseInt(item.value)
                    outputCounts = outputCounts + eachVal;
                });
                //document.getElementById(`outputs-{}`).innerHTML = outputCounts;

                // 生成饼图
                drawOutputPie('pie-data-output-' + parseInt(eqp.split('S')[1]), outputs, parseInt(eqp.split('S')[1]) +'号机-实时合盘明细')
                // 生成利用率图
                drawGuage('guage-data-utilization-' + parseInt(eqp.split('S')[1]), utilization.toFixed(2), 100, parseInt(eqp.split('S')[1]) + '号机-实时设备利用率(%)')

                //DisplayData();
            },
            error: function () {

            }
        });
    }

    function GetEQPHistoryData(eqp) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqp,//$("#workshopFilter").val(),
                "datetime": datetime,
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'RIDM/GetEQPHistoryData',
            success: function (retdata) {
                var maxUtilizationPerDay = retdata.maxUtilizationPerDay;
                var outputList = retdata.maxOutputPerDay;
             
                var dateArr = [];
                var utilizationArr = []
                maxUtilizationPerDay.forEach(item => {
                    dateArr.push(item.Date);
                    utilizationArr.push((parseFloat(item.Value) * 100).toFixed(1));
                })

                var types = ['4to1', '3to1', '2to1'];
                // 创建新的数组来存放四合一、三合一、二合一 三种Name的数据
                var resultA = [];//四合一
                var resultB = [];//三合一
                var resultC = [];//二合一
                var resultDates = [];

                // 使用reduce方法处理数据
                var result = outputList.reduce(function (acc, item) {
                    // 将数据按照日期进行分组
                    if (!acc[item.Date]) {
                        acc[item.Date] = {};
                    }

                    // 将数据按照Name存入对应的日期分组中
                    acc[item.Date][item.Name] = item.Value;

                    return acc;
                }, {});

                // 遍历日期分组
                for (var date in result) {
                    // 将日期存入新的数组中
                    resultDates.push(date);

                    // 检查4to1、3to1、2to1三种Name的数据是否存在，不存在则补充值为0
                    for (var i = 0; i < types.length; i++) {
                        if (result[date][types[i]] === undefined) {
                            switch (types[i]) {
                                case '4to1':
                                    resultA.push('0');
                                    break;
                                case '3to1':
                                    resultB.push('0');
                                    break;
                                case '2to1':
                                    resultC.push('0');
                                    break;
                            }
                        } else {
                            if (types[i] == '4to1') {
                                resultA.push(result[date][types[i]]);
                            } else if (types[i] == '3to1') {
                                resultB.push(result[date][types[i]]);
                            } else if (types[i] == '2to1') {
                                resultC.push(result[date][types[i]]);
                            }
                        }
                    }
                }
                //console.log(utilizationArr)
                var eqpNumber = parseInt(eqp.split('S')[1]);
                drawhistoryOutput("bar-data-output-" + eqpNumber, resultA, resultB, resultC, resultDates,"History Output");
                drawhistoryRate("trend-data-output-" + eqpNumber, utilizationArr, dateArr,  "Utilization Trend")
                //drawGuage('guage-data-utilization-' + eqpNumber, utilizationArr[utilizationArr.length - 1], 100, eqpNumber + '号机-设备利用率(%)')
            },
            error: function () {

            }
        });
    }
    function GetHistoryUtilization() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqid,//$("#workshopFilter").val(),
                "datetime": datetime,
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'RIDM/GetHistoryUtilization',
            success: function (retdata) {
                var maxValuesPerDay = retdata.maxValuesPerDay;
                console.log(maxValuesPerDay)
                var dateArr = [];
                var utilizationArr = []
                maxValuesPerDay.forEach(item => {
                    dateArr.push(item.Date);
                    utilizationArr.push((parseFloat(item.Value) * 100).toFixed(1));
                })
                console.log(dateArr)
                console.log(utilizationArr)
                drawhistoryRate("trend-data-output", utilizationArr, dateArr,"设备利用率趋势")
            },
            error: function () {

            }
        });
    }

    function GetHistoryOutput() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqid,//$("#workshopFilter").val(),
                "datetime": datetime,
            },
            url: 'RIDM/GetHistoryOutput',
            success: function (retdata) {
                console.log(retdata.maxValuesPerDay)
                var outputList = retdata.maxValuesPerDay;

                var types = ['4to1', '3to1', '2to1'];
                // 创建新的数组来存放四合一、三合一、二合一 三种Name的数据
                var resultA = [];//四合一
                var resultB = [];//三合一
                var resultC = [];//二合一
                var resultDates = [];

                // 使用reduce方法处理数据
                var result = outputList.reduce(function (acc, item) {
                    // 将数据按照日期进行分组
                    if (!acc[item.Date]) {
                        acc[item.Date] = {};
                    }

                    // 将数据按照Name存入对应的日期分组中
                    acc[item.Date][item.Name] = item.Value;

                    return acc;
                }, {});
                
                // 遍历日期分组
                for (var date in result) {
                    // 将日期存入新的数组中
                    resultDates.push(date);

                    // 检查4to1、3to1、2to1三种Name的数据是否存在，不存在则补充值为0
                    for (var i = 0; i < types.length; i++) {
                        if (result[date][types[i]] === undefined) {
                            switch (types[i]) {
                                case '4to1':
                                    resultA.push('0');
                                    break;
                                case '3to1':
                                    resultB.push('0');
                                    break;
                                case '2to1':
                                    resultC.push('0');
                                    break;
                            }
                        } else {
                            if (types[i] == '4to1') {
                                resultA.push(result[date][types[i]]);
                            } else if (types[i] == '3to1') {
                                resultB.push(result[date][types[i]]);
                            } else if (types[i] == '2to1') {
                                resultC.push(result[date][types[i]]);
                            }
                        }
                    }
                }

                // 输出结果
                console.log(resultA);
                console.log(resultB);
                console.log(resultC);
                drawhistoryOutput("bar-data-output", resultA, resultB, resultC, resultDates, "合盘数历史明细");
            },
            error: function () {

            }
        });
    }

    function GetTotalOutput() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "datetime": datetime,
            },
            url: 'RIDM/GetTotalOutput',
            success: function (retdata) {
                var data = retdata.data
                //console.log(retdata.data)
                var outputs = [];
                eqpdata.forEach(eqp => {
                    //console.log(eqp)
                    //GetEQPRealtimeData(eqp);
                    var arrayByEQID = data.filter(item => item.EQID == eqp);
                    //// 按 Name 分类并求和
                    const resultByName = arrayByEQID.reduce((acc, curr) => {
                        if (!acc[curr.Name]) {
                            acc[curr.Name] = { name: curr.Name, value: 0 };
                        }
                        acc[curr.Name].value += parseInt(curr.Value);
                        return acc;
                    }, {});
                    const arrayByName = Object.values(resultByName);
                    //console.log(arrayByName)
                    //arrayByEQID.forEach(item => {
                    //    var outputItem = { name: getMapping(item.Name), value: item.Value };
                    //    outputs.push(outputItem);
                    //})

                    //console.log(outputs);
                    var eqpNumber = parseInt(eqp.split('S')[1]);
                    drawOutputPie("pie-data-output-" + eqpNumber, arrayByName, "Accumulated Output")//累计合盘总数
                })
                //const resultByEQID = data.reduce((acc, curr) => {
                //    if (!acc[curr.EQID]) {
                //        acc[curr.EQID] = { name: curr.EQID, value: 0 };
                //    }
                //    acc[curr.EQID].value += parseInt(curr.Value);
                //    return acc;
                //}, {});
                //const arrayByEQID = Object.values(resultByEQID);
                //console.log(arrayByEQID)

                //// 按 Name 分类并求和
                //const resultByName = data.reduce((acc, curr) => {
                //    if (!acc[curr.Name]) {
                //        acc[curr.Name] = { name: curr.Name, value: 0 };
                //    }
                //    acc[curr.Name].value += parseInt(curr.Value);
                //    return acc;
                //}, {});
                //const arrayByName = Object.values(resultByName);
                //console.log(arrayByName)

                //drawTotalOutputPie("pie-data-totaloutput", arrayByEQID, arrayByName,"累计合盘总数(所有合料机)")
             
            },
            error: function () {

            }
        });
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
                    fontSize: 16
                }


            },
            series: [
                {
                    type: 'gauge',
                    radius: '70%',
                    center: ['50%', '60%'],
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
                        width: '70%',
                        lineHeight: 40,
                        borderRadius: 12,
                        offsetCenter: [0, '-15%'],
                        fontSize: 30,
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
                    radius: '70%',
                    center: ['50%', '60%'],
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

        var myChart = echarts.init(chartDom,'dark');
        var option;


        option = {
            title: {
                //top: '2%',
                text: name,
                left: 'left',
                textStyle: {
                    color: 'white',
                    fontSize: 16
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
                    return `${getMapping(name)}  ${tarValue}`;
                },

            },
            tooltip: {
                trigger: 'item',
                extraCssText: 'width:auto;height:auto'
            },
            series: [

                {
                    
                    type: 'pie',
                    radius: ['30%', '60%'],
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
                                return `{name|${getMapping(params.name)}}\n\n{output|${params.value}}`;
                            } else {
                                return '';
                            }


                        },
                        
                        minMargin:20,
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

    function drawTotalOutputPie(id, dataExternal, dataInner,name) {
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

        var total = 0;
        dataExternal.forEach(item => {
            total += item.value
        });
        //console.log(total);
        option = {
            title: {
                top: '3%',
                text: name + '\n\n' + total,
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
                bottom:'bottom',
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
                    name:'合盘种类',
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
                            var name = getMapping(param.name);
                            //console.log(parseInt(param.name.split('S')[1]))
                            return `${name}:${param.value}`;
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
                    name: '合料机',
                    type: 'pie',
                  
                    radius: ['50%', '70%'],
                    //itemStyle: {
                    //    borderColor: '#fff',
                    //    borderWidth: 1
                    //},
                    label: {
                        alignTo: 'edge',
                        formatter: function (param) {
                            var name = parseInt(param.name.split('S')[1]);
                            //console.log(parseInt(param.name.split('S')[1]))
                            return `${name}号机\n\n${param.value}`;
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
                top:'3%',
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

    function drawhistoryOutput(id, fourData,threeData,twoData, dateData,name) {
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
                //data: ['Time'],
                left: 'right',
                top:'3%',
                textStyle: {
                    color: "#fff"
                }
            },
            grid: {
                left: '12%',
                top: '30%',
                //height: '60%'
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
                    rotate: 25
                }
            },
            yAxis: [
                {
                    type: 'value',
                    name: '盘数',
                    position: 'left',
                   
                    min: 0,
                    max: function (value) { return Math.ceil(value.max)},
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
            series: [

                {
                    name: '4 to 1',
                    type: 'bar',
                    barHeight: '100%',
                    stack: 'total',
                    emphasis: {
                        focus: 'series'
                    },
                    data: fourData,
                    //itemStyle: {
                    //    color: 'rgba(238, 232, 170, 1)',
                    //},
                    label: {
                        show: true,
                        //rotate: 25,
                        //position: 'center',
                        //align: 'left',
                        fontSize: 13,
                        color:'white',
                        fontWeight: 'bold',
                        textStyle: {
                            textShadowColor: 'rgba(0, 0, 0, 0.5)', // 阴影颜色
                            textShadowBlur: 5 // 阴影模糊大小
                        },
                        formatter: function (params) {
                            var value = parseFloat(params.value).toFixed(0)
                            if (value > 0) {
                                return parseFloat(params.value).toFixed(0);
                            } else {
                                return '';
                            }


                        }
                        //formatter: function (params) {

                        //    return parseFloat(params.value).toFixed(0);

                        //}
                    }

                },
                {
                    name: '3 to 1',
                    type: 'bar',
                    barHeight: '100%',
                    stack: 'total',
                    label: {
                        show: false
                    },
                    emphasis: {
                        focus: 'series'
                    },
                    data: threeData,
                    //itemStyle: {
                    //    color: 'green',
                    //},
                    label: {
                        show: true,
                        //rotate: 25,
                        //position: 'center',
                        //align: 'left',
                        fontSize: 13,
                        color: 'white',
                        fontWeight: 'bold',
                        textStyle: {
                            textShadowColor: 'rgba(0, 0, 0, 0.5)', // 阴影颜色
                            textShadowBlur: 5 // 阴影模糊大小
                        },
                        formatter: function (params) {
                            var value = parseFloat(params.value).toFixed(0)
                            if (value > 0) {
                                return parseFloat(params.value).toFixed(0);
                            } else {
                                return '';
                            }
                            

                        }
                    }
                },
                {
                    name: '2 to 1',
                    type: 'bar',
                    barHeight: '100%',
                    stack: 'total',
                    label: {
                        show: false
                    },
                    emphasis: {
                        focus: 'series'
                    },
                    data: twoData,
                    //itemStyle: {
                    //    color: 'blue',
                    //},
                    label: {
                        show: true,
                        //rotate: 25,
                        //position: 'center',
                        //align: 'left',
                        fontSize: 13,
                        color: 'white',
                        fontWeight: 'bold',
                        textStyle: {
                            textShadowColor: 'rgba(0, 0, 0, 0.5)', // 阴影颜色
                            textShadowBlur: 5 // 阴影模糊大小
                        },
                        formatter: function (params) {
                            var value = parseFloat(params.value).toFixed(0)
                            if (value > 0) {
                                return parseFloat(params.value).toFixed(0);
                            } else {
                                return '';
                            }


                        }
                    }
                },
            ]
        };
        option && myChart.setOption(option);
        myChart.resize();
        myChart.on('click', function (param) {
            //console.log(param.name);

            
            //getStatusDetails(week);
        })


    }


})
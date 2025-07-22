import { switchView, getDate, clearDoms, removeObj, getSelectEQPname, showStatusCount } from './js/utils/tools.js'

layui.use(['table', 'jquery', 'form', 'laydate'], function () {
    var table = layui.table
        , form = layui.form
        , $ = layui.jquery
        , laydate = layui.laydate;

    form.render();
    form.render("select");
    window.weekArr;
    var eqpdata = [];
    let currentIndex = 0;
    var eqid;
    var selectEQP = document.getElementById("eqpFilter");
    const today = new Date();
    var datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
    $("#datepicker").val(datetime);

    const dict = {};
    addMapping("STATION_1", "Plasma Treatment")
    addMapping("STATION_2", "Dispense the Sealing Glue to FH")
    addMapping("STATION_3", "Assemble the cover to FH")
    addMapping("STATION_4", "Auto-screwing")
    addMapping("STATION_5", "Auto-unload the FH Assy")


    var interval = document.getElementById("interval");
    var intervalValue = 30;
    //轮播定时器
    var intervalID;
    //每1分钟刷新一次界面
    var defaultDataRefresh = setInterval(function () {
        RefreshRealtimeData();
    }, 60000);

    interval.addEventListener("change", function (event) {

        intervalValue = event.target.value;
        //console.log("Selected value: " + intervalValue);
        clearInterval(intervalID)
        //console.log(intervalID)
        intervalID = setInterval(DisplayData, intervalValue * 1000);
        //setInterval(DisplayData, intervalValue * 1000);
        // 在这里执行您希望执行的操作



    });
    selectEQP.addEventListener("change", function (event) {
        eqid = event.target.value;
        var eqpName = getMapping(eqid);

        console.log("Selected value: " + eqid);

        // 在这里执行操作
        document.getElementById("EQID").innerHTML = eqid;
        document.getElementById("eqpName").innerHTML = eqpName;
        
        GetEQPRealtimeData();


    });
    laydate.render({
        elem: '#datepicker'

        //, value: date
        , done: function (value, date, endDate) {

            datetime = value;


            //GetEQPRealtimeData();
            GetEQPAlarmDetailsByDate(datetime);



        }

    });

    $("#flexSwitchCheckChecked").on('change', function () {
        if (this.checked) {
            // 开启状态
            console.log('Switch is on');

            clearInterval(intervalID)
            clearInterval(defaultDataRefresh)

            datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
            $("#datepicker").val(datetime);

            // 执行相关操作
            intervalID = setInterval(DisplayData, intervalValue * 1000);
        } else {
            // 关闭状态
            console.log('Switch is off');
            // 执行相关操作
            clearInterval(intervalID);

            //defaultDataRefresh = setInterval(function () {
            //    RefreshRealtimeData();
            // }, 60000);
        }
    })


    
    //新增开线时间
    $("#addMfgTime").click(function () {
        layer.open({
            title: 'Add Manufaturing Time'
            , type: 2
            , btn: ['OK', 'Cancel']
            , content: 'ETechDashboard/setWeek'
            , area: ['45%', '55%']
            , success: function (layero, index) {
                //向layer页面传值，传值主要代码
                var body = layer.getChildFrame('body', index);
               
                body.find("[id='line']").val('ETECH');
            }
            , yes: function (index) {
                var res = window["layui-layer-iframe" + index].callback();
                var data = JSON.parse(res);
                //console.log(data);
                if (data.start >= data.end) {
                    layer.msg('错误：开线时间大于结束时间！');
                } else {
                    //console.log(data);
                    // layer.msg(res);
                    setStartEnd(data.start, data.end, data.idleduration,data.eqptype);
                    layer.close(index);
                }

            }, btn2: function (index, layero) {
                layer.msg('取消操作');
            }

        });
    });
    //更新产能指标
    $("#updateTagret").click(function () {
        layer.open({
            title: 'Set Target'
            , type: 2
            , id: 'SetTarget'
            , btn: ['OK', 'Cancel']
            , content: 'ETechDashboard/setTarget'
            , area: ['30%', '30%']
            , success: function (layero, index) {
                //向layer页面传值，传值主要代码
                //   var body = layer.getChildFrame('body', index);
                var body = layer.getChildFrame('body', index);

                body.find("[id='line']").val('ETECH');
            }
            , yes: function (index) {
                var res = window["layui-layer-iframe" + index].callback();
                var data = JSON.parse(res);

                var target = data.Target;
                var eqptype = data.eqptype;

                setTarget(target, eqptype);

                layer.close(index);
                //}

            }, btn2: function (index, layero) {
                layer.msg('取消操作');
            }

        });

    })
    $("#updateAlarmTarget").click(function () {
        layer.open({
            title: 'Set Target'
            , type: 2
            , id: 'SetTarget'
            , btn: ['OK', 'Cancel']
            , content: 'ETechDashboard/setTarget'
            , area: ['30%', '30%']
            , success: function (layero, index) {
                //向layer页面传值，传值主要代码
                //   var body = layer.getChildFrame('body', index);
                var body = layer.getChildFrame('body', index);

                body.find("[id='line']").val('ETECH');
            }
            , yes: function (index) {
                var res = window["layui-layer-iframe" + index].callback();
                var data = JSON.parse(res);

                var target = data.Target;
                var eqptype = data.eqptype;
                //console.log(target);
                setAlarmTarget(target, eqptype);

                layer.close(index);
                //}

            }, btn2: function (index, layero) {
                layer.msg('取消操作');
            }

        });

    })
    $(".exportReport").click(function () {
        //console.log(window.weekArr)
        layer.open({
            title: '下载报表'
            , type: 2
            , btn: ['确定']
            , content: 'ETechDashboard/DownloadReports'
            , area: ['35%', '45%']
            , success: function (layero, index) {
                
                ////向layer页面传值，传值主要代码
                var body = layer.getChildFrame('body', index);
                //body.find("[id='equipment']").val(e.data.equipment);
                //body.find("[id='station']").val('Station_' + (e.data.value[0] + 1));
                body.find("[id='weekFilter']").html(window.weekArr);

                // 在子页面加载完毕后，通过iframe访问子页面的window对象
                const iframeWindow = window[layero.find('iframe')[0]['name']];
                iframeWindow.dataFromParent = window.weekArr;  // 传递数据
            }
            , yes: function (index) {
                layer.close(index);
            }

        });
    });

    GetEQPList();
    
    function GetEQPList() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                
            },
            url: 'ETechDashboard/GetEQPList',
            success: function (retdata) {

                $("#eqpFilter").html(retdata.option);
                eqpdata = retdata.eqList;

                DisplayData();
                //DisplayData();
            },
            error: function () {

            }
        });
    }

    function DisplayData() {

        if (currentIndex < eqpdata.length) {
            eqid = eqpdata[currentIndex];
            var eqpName = getMapping(eqid);
            document.getElementById("EQID").innerHTML = eqid;
            document.getElementById("eqpName").innerHTML = eqpName;

            const today = new Date();
            datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
            $("#datepicker").val(datetime);

            GetEQPRealtimeData();
            GetEQPAlarmDetailsByDate(datetime);
            GenerateStatusCard();
            GetHistoryAlarmRate();
         



            currentIndex++;
        } else {
            currentIndex = 0;
            DisplayData();
        }
    }


    function RefreshRealtimeData() {
        console.log("Refresh Dashboard:" + eqid);
        GetEQPRealtimeData();
        GetEQPAlarmDetailsByDate(datetime);
        GenerateStatusCard();


    }

    /*获取机种、料号、工单号、设备利用率、合盘数*/
    function GetEQPRealtimeData() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqid,//$("#workshopFilter").val(),
                "datetime": datetime,
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'ETechDashboard/GetEQPRealtimeData',
            success: function (retdata) {
                console.log(retdata)
                var status = retdata.realtimeData.Status;
                var output = retdata.realtimeOutput;
                var outputTarget = retdata.outputTarget;

                var alarmrate = retdata.AlarmRate;
                var loadingrate = (1 - alarmrate) * 100;
                console.log(loadingrate)
                if (eqid == 'STATION_1') {
                    status = 'Run'
                }
                document.getElementById("outputs").innerHTML = output;
                document.getElementById("status").innerHTML = status;

                document.getElementById("down-rate").innerHTML = (alarmrate * 100).toFixed(2);

                var datenow = new Date().toLocaleString('chinese', { hour12: false });
                var dateDay = getDate() + " 09:00:00";

                var dateNight = getDate() + " 21:00:00";
                var OEEvalue;

                if (new Date(dateDay) <= new Date(datenow) && new Date(dateNight) >= new Date(datenow)) {
                    var timespan = new Date(datenow).getTime() - new Date(dateDay).getTime()
                    var totalspan = new Date(dateNight).getTime() - new Date(dateDay).getTime()

                    var realtimeTarget = parseFloat(outputTarget) * (timespan / totalspan)
                    if (realtimeTarget == 0) {
                        realtimeTarget = parseFloat(outputTarget)
                    }

                    OEEvalue = (output / realtimeTarget) * 100;

                } else {

                    OEEvalue = (output / parseFloat(outputTarget)) * 100;

                }
                if (OEEvalue > 100) {
                    console.log("OEE exceed 100: " + OEEvalue);
                    OEEvalue = 100
                }

                drawGuage("loading-guage", loadingrate.toFixed(2), 100, "Loading Rate \n\n" + datetime, "%")
                drawGuage("achieve-guage", OEEvalue.toFixed(2), 100, "Achieve Rate \n\n" + datetime, "%")
                drawGuage("output-guage", output, outputTarget, "Output\n\n" + datetime, "")
                // 生成点阵图
                // drawOutputScatter('scatter-data-ct', outputArr, "CT - " + retdata.startTime + "~" + retdata.endTime)

                switch (status) {
                    case "Idle":
                        //document.getElementById("status-icon").removeAttribute("class")

                        document.getElementById("status-icon").className = "fa fa-solid  fa-exclamation-triangle fa-xl col";
                        document.getElementById("status-icon").style.color = "yellow";

                        break;
                    case "Run":
                        document.getElementById("status-icon").className = "fa fa-solid fa-check-circle fa-xl col";
                        document.getElementById("status-icon").style.color = "green";
                        break;
                    case "Alarm":
                    case "Offline":
                        document.getElementById("status-icon").className = "fa fa-solid  fa-exclamation-triangle fa-xl col";
                        document.getElementById("status-icon").style.color = "red";
                        break;
                }
            },
            error: function () {

            }
        });
    }
    
    function GetHistoryAlarmRate() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "eqpType": 'ETECH'
            },
            url: 'ETechDashboard/GetHistoryAlarmRate',//ACCBoxDashboard
            success: function (retdata) {
                var monthlyData = retdata.alarmMonArr;
                const months = monthlyData.map(obj => obj.time)
                const monthAlarms = monthlyData.map(obj => obj.alarmtimes)
                const monthMfg = monthlyData.map(obj => obj.mfgtimes - obj.alarmtimes)
                const monthRate = monthlyData.map(obj => (obj.alarmtimes / obj.mfgtimes) * 100)
                var target = retdata.target;
                window.weekArr = retdata.weekOption
                console.log(window.weekArr)
                //monthlyAlarmRateChart = gethistoryRate('monthly-alarm-rate', monthRate, months, 'Monthly', target);
                // gethistoryTime('monthly-alarm-time', monthAlarms, months, monthMfg, 'Monthly');
                gethistoryAlarmRate('monthly-alarm-time', monthRate, months, target)

            },
            error: function () {

            }
        });
    }

  
    function GetEQPAlarmDetailsByDate(selectDate) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqid,
                "datetime": selectDate
            },
            url: 'ETechDashboard/GetEQPAlarmDetailsByDate',
            success: function (retdata) {

                //var alarmCodes = retdata.alarmtotal;
                var alarmList = retdata.alarmList;
                //console.log(alarmList)
                //drawhistoryAlarmDetails("bar-data-alarmcode", alarmCodes, "Alarm Codes - "+selectDate)
                layui.table.render({
                    elem: '#table-data-alarms'
                    //, width: '100%'   // 固定表格宽度
                    , cols: [[ //标题栏
                        { field: 'AlarmEqp', title: 'EQID', align: "center", width: '20%' }
                        , { field: 'AlarmTime', title: 'Alarm Time', templet: '<div>{{ FormDate(d.AlarmTime, "MM-dd HH:mm:ss") }}</div>', align: "center", width: "15%", unresize: true }
                        /*, { field: 'CA', title: 'Carrier ID', align: "center", width: '15%' }*/
                        , { field: 'AlarmCode', title: 'Code', align: "center", width: "15%", unresize: true }
                        , { field: 'AlarmText', title: 'Text', align: "center", width: "50%", unresize: true }


                    ]]
                    , data: alarmList
                    , size: 'sm'
                    //, scrollX: true // 开启横向滚动
                    , loading: false
                    , limit: 10
                    , done: function (res, curr, count) {


                    }


                });


            },
            error: function () {
                alert("error!");
            }
        });
    }

    function GenerateStatusCard() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {

            },
            url: 'ETechDashboard/GetTotalRealtimeData',
            success: function (retdata) {
                var data = retdata.mergeList;
                //console.log(data)

                const result = data.reduce((acc, item) => {
                    acc[item.Status] = (acc[item.Status] || 0) + 1;
                    return acc;
                }, {});

                //document.getElementById("AlarmTotal").innerHTML = result.Alarm == undefined ? "0":result.Alarm;
                //document.getElementById("IdleTotal").innerHTML = result.Idle == undefined ? "0" : result.Idle;
                //document.getElementById("RunTotal").innerHTML = result.Run == undefined ? "0" : result.Run;
                //console.log(result);
                document.getElementById("divStationCapacity1").innerText = 0;
                document.getElementById("divStationCapacity2").innerText = 0;
                document.getElementById("divStationCapacity3").innerText = 0;
                document.getElementById("divStationCapacity4").innerText = 0;
                document.getElementById("divStationCapacity5").innerText = 0;
                for (var i = 0; i < data.length; i++) {

                    GenerateStationProfileDetails(data[i]);
                   

                }
            },
            error: function () {

            }
        });
    }

    function GenerateStationProfileDetails(data) {
        var index = data["EQID"].split('_')[1];
        var objaStationCapacity = document.getElementById("aStationCapacity" + index);
        var objiStationCapacity = document.getElementById("iStationCapacity" + index);
        var objdivStationCapacity = document.getElementById(`divStationCapacity${index}`);

        if (data["Status"] == "Alarm") {
            //objaStationCapacity5.attributes.class = "btn border-indigo-400 text-indigo-400 btn-flat btn-rounded btn-icon btn-xs valign-text-bottom";
            objiStationCapacity.className = "fa fa-solid fa-exclamation-triangle fa-xl col-4";
            objiStationCapacity.style.color = "red";
        } else if (data["Status"] == "Idle") {
            objiStationCapacity.className = "fa fa-solid fa-exclamation-triangle fa-xl col-4";
            objiStationCapacity.style.color = "yellow";
        }

        objdivStationCapacity.innerText = data["Output"];
    }

    function drawGuage(id, data, maxdata, name, unit) {

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
                bottom: '6%',
                left: 'center',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }


            },
            series: [
                {
                    type: 'gauge',
                    radius: '60%',
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
                        fontSize: 18
                    },
                    anchor: {
                        show: false
                    },
                    title: {
                        show: false

                    },
                    detail: {
                        valueAnimation: true,
                        width: '60%',
                        lineHeight: 40,
                        borderRadius: 12,
                        offsetCenter: [0, '-15%'],
                        fontSize: 30,
                        fontWeight: 'bolder',
                        formatter: '{value}' + unit,
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
                    radius: '60%',
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

    function gethistoryTime(id, timeData, weekData, rundata, time) {
        

        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        //console.log(chartDom.style.width)
        //console.log(chartDom.style.height)
        var myChart = echarts.init(chartDom, 'dark');
        var option;

        option = {
            title: {
                text: time + "-Alarm Time",
                textStyle: {
                    color: "#fff",
                    fontSize: 11
                }
            },

            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'shadow'
                }
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
                textStyle: {
                    color: "#fff"
                }
            },
            grid: {
                left: '12%',
                top: '24%',
                height: '60%'
            },
            xAxis: {
                type: 'category',
                axisTick: {
                    alignWithLabel: true
                },
                data: weekData,
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
                    name: 'Time(min)',
                    position: 'left',
                    min: 0,
                    max: function (value) { return Math.ceil(value.max) * 0.6 },
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
                    name: 'Alarm',
                    type: 'bar',
                    barHeight: '100%',
                    stack: 'total',
                    emphasis: {
                        focus: 'series'
                    },
                    data: timeData,
                    itemStyle: {
                        color: 'rgba(238, 232, 170, 1)',
                    },
                    label: {
                        show: true,
                        rotate: 25,
                        position: 'top',
                        align: 'left',
                        color: 'white',
                        formatter: function (params) {

                            return parseFloat(params.value).toFixed(1)

                        }
                    }

                },
                {
                    name: 'Run&Idle',
                    type: 'bar',
                    barHeight: '100%',
                    stack: 'total',
                    label: {
                        show: false
                    },
                    emphasis: {
                        focus: 'series'
                    },
                    data: rundata,
                    itemStyle: {
                        color: 'rgba(255,255,255,0.3)',
                    },
                },

            ]
        };
        option && myChart.setOption(option);
        myChart.resize();
        myChart.on('click', function (param) {
            //console.log(param.name);

            getLatestAlarmRate(param.name);
            window.week = param.name;
            //getStatusDetails(week);
        })


    }

    function gethistoryAlarmRate(id, rateData, trenddates, alarmtarget) {
        var chartDom = document.getElementById(id);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        //console.log(chartDom.style.width)
        //console.log(chartDom.style.height)
        var myChart = echarts.init(chartDom, 'dark');

        var option;

        option = {
            title: {
                text: "Monthly Alarm Rate",
                textStyle: {
                    color: "#fff"
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
            grid: {},
            xAxis: {
                type: 'category',
                axisTick: {
                    alignWithLabel: true
                },
                data: trenddates,
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    }
                },
                axisLabel: {
                    rotate: 25,
                    fontSize: 15,
                }
            },
            yAxis: [
                {
                    type: 'value',
                    name: 'Rate',
                    position: 'left',
                    min: 0,
                    max: function (value) {
                        if (value.max < alarmtarget) { return Math.ceil(alarmtarget * 1.5) } else {
                            return Math.ceil(value.max * 1.5)
                        } },
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
                        fontSize: 15,
                    }
                }],
            series: [
                {
                    name: 'Rate',
                    type: 'line',
                    data: rateData,
                    areaStyle: {
                        color: '#BC8F8F',//'#339933',
                    },
                    smooth: true,
                    label: {
                        show: true,
                        position: 'top',
                        color: 'white',
                        formatter: function (params) {
                            return parseFloat(params.value).toFixed(2) + '%'
                        }
                    },
                    lineStyle: {
                        color: 'yellow'
                    },
                    markLine: {
                        silent: true,
                        itemStyle: {
                            normal: {
                                color: '#FA8565',
                            }
                        },
                        data: [{
                            yAxis: alarmtarget,
                            label: {
                                color: 'white',
                                formatter: function (params) {
                                    return parseFloat(params.value).toFixed(0) + '%'
                                }
                            }
                        }]
                    }
                }
            ]
        };
        option && myChart.setOption(option);
        myChart.resize();
        myChart.on('click', function (param) {
            //console.log(param.name);

            getLatestAlarmRate(param.name);
            week = param.name;
            //getStatusDetails(week);
        })

    }
    function setTarget(target, eqptype) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "target": target,
                "eqpType": eqptype
            },
            url: 'ETechDashboard/SetUPD',//ACCBoxDashboard/
            success: function (retdata) {


                location.reload();


            },
            error: function () {

            }
        });

    }

    function setAlarmTarget(target, eqptype) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "target": target,
                "eqpType": eqptype
            },
            url: 'ETechDashboard/SetAlarmRateTarget',//ACCBoxDashboard/
            success: function (retdata) {


                location.reload();


            },
            error: function () {

            }
        });

    }
    function setStartEnd(starttime, endtime, idleduration, eqpType) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "starttime": starttime,
                "endtime": endtime,
                "idleduration": idleduration,
                "eqpType": eqpType
            },
            url: 'ETechDashboard/SetStartEnd',//ACCBoxDashboard/
            success: function (retdata) {
                var eqdata = retdata.eqdata;
                var alarmrates = retdata.alarmrates;
                var starttime = retdata.starttime;
                var endtime = retdata.endtime;
                var alarmtimes = retdata.alarmtimes;
                var duration = starttime + "~" + endtime;
                var maxtime = retdata.maxtime;
                var maxrate = retdata.maxrate;
                //gettrendchart('trendchart', alarmrates, eqdata, alarmtimes, duration, maxtime, maxrate);
            },
            error: function () {

            }
        });

    }

    // 添加映射关系
    function addMapping(key, value) {

        dict[key] = value;
    }


    // 获取映射结果
    function getMapping(key) {
        return dict[key] || '';
    }

 
})
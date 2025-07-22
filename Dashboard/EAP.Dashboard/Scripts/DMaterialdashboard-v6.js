layui.use(['table', 'jquery', 'form', 'laydate'], function () {
    var table = layui.table
        , form = layui.form
        , $ = layui.jquery
        , laydate = layui.laydate;

    form.render();
    form.render("select");
    var eqpdata = [];
    let currentIndex = 0;
    var eqid;
    var selectEQP = document.getElementById("eqpFilter");
    var selectPN = document.getElementById("pnFilter");

    var outputArr = [];

    const today = new Date();
    const sevenDaysAgo = new Date(today);
    sevenDaysAgo.setDate(today.getDate() - 7);

    var datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
    var starttime = sevenDaysAgo.getFullYear() + '-' +
        (sevenDaysAgo.getMonth() + 1).toString().padStart(2, '0') + '-' +
        sevenDaysAgo.getDate().toString().padStart(2, '0');
    $("#datepicker").val(datetime);
    $("#startdatepicker").val(starttime);


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
        console.log("Selected value: " + eqid);

        // 在这里执行操作
        document.getElementById("EQID").innerHTML = eqid;


        try {
            var paramByDate = {
                "EQID": eqid,
                "datetime": datetime,
            };
            var paramHistory = {
                "EQID": eqid,
                "datetime": datetime,
                "starttime": starttime
            };


            var requests = [
                fetchData('DMaterialDashboard/GetEQPRealtimeData', paramByDate),
                fetchData('DMaterialDashboard/GetEQPAlarmDetailsByDate', paramByDate),
                fetchData('DMaterialDashboard/GetEQPRunrateByDate', paramByDate),
                fetchData('DMaterialDashboard/GetEQPDownrateByDate', paramByDate),

                fetchData('DMaterialDashboard/GetEQPHisRunrate', paramHistory),
                fetchData('DMaterialDashboard/GetEQPHisDownRate', paramHistory),

                fetchData('DMaterialDashboard/GetEQPHisDataOutput', paramHistory),
                fetchData('DMaterialDashboard/GetEQPHisDataAlarm', paramHistory),
            ];
            var spinner = document.createElement('div');
            spinner.id = 'loading'
            spinner.className = 'spinner-border text-light'
            spinner.removeAttribute('hidden')
            document.body.appendChild(spinner)
            Promise.all(requests)
                .then((responses) => {

                    responses.forEach((response, index) => {
                        //console.log(response)
                        // 处理每个响应
                        //const url = requests[index].url;  // 获取对应的 URL
                        handleResponse(response.url, response.response);
                    });
                    document.body.removeChild(spinner)
                })
                .catch((error) => {
                    console.error('请求失败:', error);
                });
        } catch (error) {
            console.error('请求失败:', error);
        }



    });

    selectPN.addEventListener("change", function (event) {
        var selectedPN = event.target.value;
        console.log("Selected PN: " + selectedPN);
        switch (selectedPN) {
            case "all":
                GetEQPRealtimeData();
                break;
            default:
                var filteredOutput = outputArr.filter(it => it.value[2] == selectedPN);
                const ctArr = filteredOutput.map(item => parseFloat(item.value[1]))


                var ctAvg = ctArr.reduce((acc, curr) => acc + curr, 0) / ctArr.length;

                // 生成点阵图
                drawOutputScatter('scatter-data-ct', filteredOutput, `Avg CT = ${ctAvg.toFixed(2)} sec - ` + selectedPN)
                break;
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

            defaultDataRefresh = setInterval(function () {
                RefreshRealtimeData();
            }, 60000);
        }
    })


    laydate.render({
        elem: '#datepicker'

        //, value: date
        , done: function (value, date, endDate) {



            if (value < starttime) {
                layer.alert("开始时间不可小于结束时间！")
                $("#datepicker").val(datetime);
                return;
            }
            datetime = value;

          
            try {
                var paramByDate = {
                    "EQID": eqid,
                    "datetime": datetime,
                };
                var paramHistory = {
                    "EQID": eqid,
                    "datetime": datetime,
                    "starttime": starttime
                };

                var requests = [
                    fetchData('DMaterialDashboard/GetEQPRealtimeData', paramByDate),
                    fetchData('DMaterialDashboard/GetEQPAlarmDetailsByDate', paramByDate),
                    fetchData('DMaterialDashboard/GetEQPRunrateByDate', paramByDate),
                    fetchData('DMaterialDashboard/GetEQPDownrateByDate', paramByDate),

                    fetchData('DMaterialDashboard/GetEQPHisRunrate', paramHistory),
                    fetchData('DMaterialDashboard/GetEQPHisDownRate', paramHistory),

                    fetchData('DMaterialDashboard/GetEQPHisDataOutput', paramHistory),
                    fetchData('DMaterialDashboard/GetEQPHisDataAlarm', paramHistory),
                ];
                var spinner = document.createElement('div');
                spinner.id = 'loading'
                spinner.className = 'spinner-border text-light'
                spinner.removeAttribute('hidden')
                document.body.appendChild(spinner)
                Promise.all(requests)
                    .then((responses) => {
                        responses.forEach((response, index) => {
                            //console.log(response)
                            // 处理每个响应
                            //const url = requests[index].url;  // 获取对应的 URL
                            handleResponse(response.url, response.response);
                        });
                        document.body.removeChild(spinner)
                    })
                    .catch((error) => {
                        console.error('请求失败:', error);
                    });

            } catch (error) {
                console.log(error)
            } 


        }

    });
    laydate.render({
        elem: '#startdatepicker'

        //, value: date
        , done: function (value, date, endDate) {

            if (datetime < value) {
                layer.alert("开始时间不可小于结束时间！")
                $("#startdatepicker").val(starttime);
                return;
            }

            starttime = value;

           
            try {
                var paramHistory = {
                    "EQID": eqid,
                    "datetime": datetime,
                    "starttime": starttime
                };

                var requests = [

                    fetchData('DMaterialDashboard/GetEQPHisRunrate', paramHistory),
                    fetchData('DMaterialDashboard/GetEQPHisDownRate', paramHistory),

                    fetchData('DMaterialDashboard/GetEQPHisDataOutput', paramHistory),
                    fetchData('DMaterialDashboard/GetEQPHisDataAlarm', paramHistory),
                ];
                var spinner = document.createElement('div');
                spinner.id = 'loading'
                spinner.className = 'spinner-border text-light'
                spinner.removeAttribute('hidden')
                document.body.appendChild(spinner)
                Promise.all(requests)
                    .then((responses) => {
                        responses.forEach((response, index) => {
                            //console.log(response)
                            // 处理每个响应
                            //const url = requests[index].url;  // 获取对应的 URL
                            handleResponse(response.url, response.response);
                        });
                        document.body.removeChild(spinner)
                    })
                    .catch((error) => {
                        console.error('请求失败:', error);
                    });
            } catch (error) {
                console.log(error)
            } 


            //GetEQPHisDataAlarm();
            //GetEQPHisDataOutput();

            //GetEQPHisRunrate();
            //GetEQPHisDownRate();
            //GetEQPHisSuccessRate();


        }

    });

    $("#download-report").click(function () {
        layer.open({
            title: '下载报表'
            , type: 2
            , btn: ['关闭']
            , content: 'DMaterialDashboard/DownloadReports'
            , area: ['55%', '60%']
            , success: function (layero, index) {
                ////向layer页面传值，传值主要代码
                var body = layer.getChildFrame('body', index);

                body.find("[id='dateFilter']").val($("#datepicker").val());
                body.find("[id='startdateFilter']").val($("#startdatepicker").val());
            }
            , yes: function (index) {
                layer.close(index);
            }

        });
    })

    GetEQPList();




    function fetchData(url, data) {
        return $.ajax({
            type: 'post',
            dataType: 'json',
            data: data,
            url: url
        }).then(response => ({ url, response }));
    }

    // 定义处理函数
    function handleResponse(url, response) {


        switch (url) {
            case 'DMaterialDashboard/GetEQPList':
                // 处理 EQP List 的响应
                $("#eqpFilter").html(response.option);
                eqpdata = response.eqList;
                DisplayData();
                break;

            case 'DMaterialDashboard/GetEQPRealtimeData':
                var outputs = response.output;
                var status = response.status;
                var successrate = response.SuccessRate * 1
                document.getElementById("success-rate").innerHTML = successrate.toFixed(2) + "%";


                outputArr = [];
                outputs.forEach(item => {
                    // 使用正则表达式提取时间戳部分
                    const timestamp = parseInt(item.UpdateTime.match(/\d+/)[0]);
                    // 将时间戳转换成 JavaScript Date 对象
                    const date = new Date(timestamp);
                    outputArr.push({
                        value: [date, item.CT, item.PN, item.ReelID],

                    });

                });


                var outputCounts = outputs.length;


                const ctArr = outputs.map(item => parseFloat(item.CT))


                var ctAvg = ctArr.reduce((acc, curr) => acc + curr, 0) / ctArr.length;


                $("#pnFilter").html(response.option);
                document.getElementById("outputs").innerHTML = outputCounts;
                document.getElementById("status").innerHTML = status;

                // 生成点阵图
                drawOutputScatter('scatter-data-ct', outputArr, `Avg CT = ${ctAvg.toFixed(2)} sec - ` + response.startTime + "~" + response.endTime)

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
                break;
            case 'DMaterialDashboard/GetEQPAlarmDetailsByDate':

                var alarmCodes = response.alarmtotal;
                var alarmList = response.alarmList;
                //console.log(alarmList)
                drawhistoryAlarmDetails("bar-data-alarmcode", alarmCodes, "Alarm Codes - " + datetime)
                layui.table.render({
                    elem: '#table-data-alarms'
                    //, width: '100%'   // 固定表格宽度
                    , cols: [[ //标题栏
                        /* { field: 'AlarmEqp', title: 'EQID', align: "center", width: '20%' }*/
                        { field: 'AlarmTime', title: 'Alarm Time', templet: '<div>{{ FormDate(d.AlarmTime, "MM-dd HH:mm:ss") }}</div>', align: "center", width: "25%", unresize: true }
                        /*, { field: 'CA', title: 'Carrier ID', align: "center", width: '15%' }*/
                        , { field: 'AlarmCode', title: 'Code', align: "center", width: "20%", unresize: true }
                        , { field: 'AlarmText', title: 'Text', align: "center", width: 0, unresize: true }


                    ]]
                    , data: alarmList
                    , size: 'sm'
                    //, scrollX: true // 开启横向滚动
                    , loading: false
                    , limit: 8
                    , done: function (res, curr, count) {


                    }


                });
                break;
            case 'DMaterialDashboard/GetEQPRunrateByDate':
                document.getElementById("loading-rate").innerHTML = response.loadingRate.toFixed(2) + "%";
                break;
            case 'DMaterialDashboard/GetEQPDownrateByDate':
                var downrate = response.DownRate * 100
                document.getElementById("down-rate").innerHTML = downrate.toFixed(2) + "%";
                break;
            case 'DMaterialDashboard/GetEQPHisRunrate':
                var runrateArr = response.runrateList;
                drawhistoryRate("trend-data-runtime", runrateArr, runrateArr.map(it => it.name), "History Loading Rate")

                break;
            case 'DMaterialDashboard/GetEQPHisDownRate':
                var totalAlarmDurationPerDay = response.totalAlarmDurationPerDay;
                //console.log(totalAlarmDurationPerDay)
                drawhistoryDownRate("trend-data-downtime", totalAlarmDurationPerDay.map(it => it.value * 100), totalAlarmDurationPerDay.map(it => it.name), "History Down Rate")
                break;
            case 'DMaterialDashboard/GetEQPHisDataOutput':
                var historyOutput = response.result;
                drawhistoryOutput("bar-data-output", historyOutput, "History Output")

                var success_fail_list = response.success_fail_list;
                drawhistoryRate("trend-success-rate", success_fail_list.map(it => it.SuccessRatio), success_fail_list.map(it => it.Date), "拆料成功率")
                break;
            case 'DMaterialDashboard/GetEQPHisDataAlarm':
                var historyAlarms = response.result;
                drawhistoryAlarm("bar-data-totalalarm", historyAlarms, "History Alarm Times")
                break;
            // 可以继续添加更多的 case 以处理不同的请求
            default:
                console.warn('未知请求:', url);
                break;
        }


    }

    function GetEQPList() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                //"workshopFilter": $("#workshopFilter").val(),
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'DMaterialDashboard/GetEQPList',
            success: function (retdata) {

                $("#eqpFilter").html(retdata.option);
                eqpdata = retdata.eqList;

                DisplayData();
                //DisplayData();
            },
            error: function () {
                document.body.removeChild(spinner)

            }
        });
    }

    async function DisplayData() {

        if (currentIndex < eqpdata.length) {
            eqid = eqpdata[currentIndex];
            document.getElementById("EQID").innerHTML = eqid;


            const today = new Date();
            datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
            $("#datepicker").val(datetime);


            //$("#loading").show();
            const isExist = document.getElementById('loading');
            if (isExist) {
                document.body.removeChild(isExist)
            }
            var spinner = document.createElement('div');
            spinner.id = 'loading'
            spinner.className = 'spinner-border text-light'
            spinner.removeAttribute('hidden')
            document.body.appendChild(spinner)
            try {
                var paramByDate = {
                    "EQID": eqid,
                    "datetime": datetime,
                };
                var paramHistory = {
                    "EQID": eqid,
                    "datetime": datetime,
                    "starttime": starttime
                };

                var requests = [
                    fetchData('DMaterialDashboard/GetEQPRealtimeData', paramByDate),
                    fetchData('DMaterialDashboard/GetEQPAlarmDetailsByDate', paramByDate),
                    fetchData('DMaterialDashboard/GetEQPRunrateByDate', paramByDate),
                    fetchData('DMaterialDashboard/GetEQPDownrateByDate', paramByDate),

                    fetchData('DMaterialDashboard/GetEQPHisRunrate', paramHistory),
                    fetchData('DMaterialDashboard/GetEQPHisDownRate', paramHistory),

                    fetchData('DMaterialDashboard/GetEQPHisDataOutput', paramHistory),
                    fetchData('DMaterialDashboard/GetEQPHisDataAlarm', paramHistory),
                ];

                await Promise.all(requests)
                    .then((responses) => {
                        responses.forEach((response, index) => {
                            //console.log(response)
                            // 处理每个响应
                            //const url = requests[index].url;  // 获取对应的 URL
                            handleResponse(response.url, response.response);
                        });

                    })
                    .catch((error) => {
                        console.error('请求失败:', error);
                    });
            } catch (error) {
                console.error('请求失败:', error);
            } finally {
                document.body.removeChild(spinner)

            }

            currentIndex++;
        } else {
            currentIndex = 0;
            DisplayData();
        }
    }


    async function RefreshRealtimeData() {
        console.log("Refresh Dashboard:" + eqid);
        const today = new Date();
        var todayStr = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0')
        if (datetime != todayStr) {
            datetime = todayStr
            $("#datepicker").val(datetime);
        }


        var paramByDate = {
            "EQID": eqid,
            "datetime": datetime,
        };

        var requests = [
            fetchData('DMaterialDashboard/GetEQPRealtimeData', paramByDate),
            fetchData('DMaterialDashboard/GetEQPAlarmDetailsByDate', paramByDate),
            fetchData('DMaterialDashboard/GetEQPRunrateByDate', paramByDate),
            fetchData('DMaterialDashboard/GetEQPDownrateByDate', paramByDate),
        ];

        await Promise.all(requests)
            .then((responses) => {
                responses.forEach((response, index) => {
                    //console.log(response)
                    // 处理每个响应
                    //const url = requests[index].url;  // 获取对应的 URL
                    handleResponse(response.url, response.response);
                });

            })
            .catch((error) => {
                console.error('请求失败:', error);
            });


    }

    function GetEQPRealtimeData() {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqid,//$("#workshopFilter").val(),
                "datetime": datetime,
                //"lineFilter": $("#lineFilter").val(),
            },
            url: 'DMaterialDashboard/GetEQPRealtimeData',
            success: function (retdata) {
                var outputs = retdata.output;
                var status = retdata.status;
                var successrate = retdata.SuccessRate * 1
                document.getElementById("success-rate").innerHTML = successrate.toFixed(2) + "%";


                outputArr = [];
                outputs.forEach(item => {
                    // 使用正则表达式提取时间戳部分
                    const timestamp = parseInt(item.UpdateTime.match(/\d+/)[0]);
                    // 将时间戳转换成 JavaScript Date 对象
                    const date = new Date(timestamp);
                    outputArr.push({
                        value: [date, item.CT, item.PN, item.ReelID],

                    });

                });
                /*console.log(outputArr)*/

                var outputCounts = outputs.length;


                const ctArr = outputs.map(item => parseFloat(item.CT))
                console.log(ctArr)

                var ctAvg = ctArr.reduce((acc, curr) => acc + curr, 0) / ctArr.length;
                console.log(ctAvg)

                $("#pnFilter").html(retdata.option);
                document.getElementById("outputs").innerHTML = outputCounts;
                document.getElementById("status").innerHTML = status;

                // 生成点阵图
                drawOutputScatter('scatter-data-ct', outputArr, `Avg CT = ${ctAvg.toFixed(2)} sec - ` + retdata.startTime + "~" + retdata.endTime)

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
    function GetEQPAlarmDetailsByDate(selectDate) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqid,
                "datetime": selectDate
            },
            url: 'DMaterialDashboard/GetEQPAlarmDetailsByDate',
            success: function (retdata) {

                var alarmCodes = retdata.alarmtotal;
                var alarmList = retdata.alarmList;
                //console.log(alarmList)
                drawhistoryAlarmDetails("bar-data-alarmcode", alarmCodes, "Alarm Codes - " + selectDate)
                layui.table.render({
                    elem: '#table-data-alarms'
                    //, width: '100%'   // 固定表格宽度
                    , cols: [[ //标题栏
                        /* { field: 'AlarmEqp', title: 'EQID', align: "center", width: '20%' }*/
                        { field: 'AlarmTime', title: 'Alarm Time', templet: '<div>{{ FormDate(d.AlarmTime, "MM-dd HH:mm:ss") }}</div>', align: "center", width: "25%", unresize: true }
                        /*, { field: 'CA', title: 'Carrier ID', align: "center", width: '15%' }*/
                        , { field: 'AlarmCode', title: 'Code', align: "center", width: "20%", unresize: true }
                        , { field: 'AlarmText', title: 'Text', align: "center", width: 0, unresize: true }


                    ]]
                    , data: alarmList
                    , size: 'sm'
                    //, scrollX: true // 开启横向滚动
                    , loading: false
                    , limit: 8
                    , done: function (res, curr, count) {


                    }


                });


            },
            error: function () {
                alert("error!");
            }
        });
    }

    function drawOutputScatter(id, data, name) {
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
                top: '2%',
                text: name,
                left: 'center',
                textStyle: {
                    color: 'white',
                    fontSize: 18
                }
            },
            //legend: {
            //    bottom: '3%',
            //    left: 'left',
            //    orient: 'vertical',
            //    textStyle: {
            //        color: 'white',
            //        fontSize: 15
            //    },

            //},
            tooltip: {
                trigger: 'item',
                extraCssText: 'width:auto;height:auto',
                formatter: function (params) {

                    return params.marker + "CT: " + params.value[1] + "s" + "<br />" + "PN: " + params.value[2] + "<br />" + "Reel ID: " + params.value[3] + "<br />" + "EventTime:" + params.value[0];

                }
            },
            dataZoom: [
                {
                    type: 'inside'
                }
                , {
                    type: 'slider',
                    showDataShadow: false,
                    /*: '93%',*/
                    height: 10,
                    handleIcon: 'M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z',
                    handleSize: 10,
                    handleStyle: {
                        color: 'white',
                        shadowBlur: 3,
                        shadowColor: 'rgba(0, 0, 0, 0.6)',
                        shadowOffsetX: 2,
                        shadowOffsetY: 2
                    },
                }
            ],
            grid: [
                {
                    x: '3.5%',
                    /*y: '10px',*/
                    width: '95%',
                    /*height: '75%'*/
                },
            ],
            xAxis: {
                type: 'time',
                splitNumber: 1,
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#fff'
                    }
                },
                axisTick: {
                    show: false
                },
            },
            yAxis: {
                //splitNumber: 10,
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: '#fff'
                    }
                },
                axisTick: {
                    show: false
                },
            },
            series: [

                {
                    type: 'scatter',
                    symbolSize: 6,
                    data: data,
                    legendHoverLink: true,
                    hoverAnimation: true,
                    emphasis: {
                        label: {
                            show: true,
                            formatter: function (param) {
                                return param.data[0];
                            },
                            position: 'top'
                        }
                    },
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
                    fontSize: 18
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
                /* top: '18%'*/
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
                    /*   fontSize: 15,*/
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
                        fontSize: 15,
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
                        fontSize: 15,
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

                }
            ]
        };
        option && myChart.setOption(option);
        myChart.resize();


    }
    function drawhistoryDownRate(id, rateData, dateData, name) {

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
                /* top: '18%'*/
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
                    /*fontSize: 15,*/
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
                        rotate: 25,
                        fontSize: 15,
                        position: 'top',
                        align: 'left',
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
                            yAxis: 0.5,
                            label: {
                                color: 'white',
                            }
                        }]
                    }
                }
            ]
        };
        option && myChart.setOption(option);
        myChart.resize();


    }

    function drawhistoryOutput(id, data, name) {
        var dates = [...new Set(data.map(item => item.name))];
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
                //data: ['Time'],
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
                    name: '盘数',
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
            series: [

                {
                    name: '盘数',
                    type: 'bar',
                    barHeight: '100%',

                    emphasis: {
                        focus: 'series'
                    },
                    data: data,
                    //itemStyle: {
                    //    color: 'rgba(238, 232, 170, 1)',
                    //},
                    label: {
                        show: true,
                        //rotate: 25,
                        //position: 'center',
                        //align: 'left',
                        fontSize: 14,
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
                        //formatter: function (params) {

                        //    return parseFloat(params.value).toFixed(0);

                        //}
                    }

                }

            ]
        };
        option && myChart.setOption(option);
        myChart.resize();
        myChart.on('click', function (param) {
            //console.log(param.name);


            //getStatusDetails(week);
        })


    }

    function drawhistoryAlarm(id, data, name) {
        var dates = [...new Set(data.map(item => item.name))];
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
                //data: ['Time'],
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
                    name: 'Times',
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
            series: [

                {
                    name: 'Times',
                    type: 'bar',
                    barHeight: '100%',

                    emphasis: {
                        focus: 'series'
                    },
                    data: data,
                    //itemStyle: {
                    //    color: 'rgba(238, 232, 170, 1)',
                    //},
                    label: {
                        show: true,
                        //rotate: 25,
                        //position: 'center',
                        //align: 'left',
                        fontSize: 14,
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

                }

            ]
        };
        option && myChart.setOption(option);
        myChart.resize();
        myChart.on('click', function (param) {
            console.log(param.name);
            GetEQPAlarmDetailsByDate(param.name)
        })

    }
    function drawhistoryAlarmDetails(id, data, name) {
        var codes = data.map(item => item.AlarmCode);//[...new Set(data.map(item => item.AlarmCode))];
        var valus = data.map(item => item.Counts);
        var texts = data.map(item => item.AlarmText);
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
            //dataZoom: [
            //    {
            //        id: 'dataZoomX',
            //        type: 'slider',
            //        xAxisIndex: [0],
            //        filterMode: 'filter'
            //    },
            //    {
            //        id: 'dataZoomY',
            //        type: 'slider',
            //        yAxisIndex: [0],
            //        filterMode: 'empty'
            //    }
            //],
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
                extraCssText: 'width:auto;height:auto',

                formatter: function (param) {

                    var codetext = '';
                    //int i
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i].AlarmCode;

                        if (item == param[0].name) codetext = data[i].AlarmText + '<br/>' + param[0].data + '次';
                    }
                    return codetext;
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
                top: '3%',
                textStyle: {
                    color: "#fff"
                }
            },
            grid: {
                //left: '3%',    // 左边距
                //right: '3%',   // 右边距
                //top: '3%',      // 上边距
                //bottom: '3%',   // 下边距

                ////height: '60%'
            },
            xAxis: {
                type: 'category',
                axisTick: {
                    alignWithLabel: true
                },
                data: codes,
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
                    name: 'Times',
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
            series: [

                {
                    name: 'Times',
                    type: 'bar',
                    barHeight: '100%',

                    emphasis: {
                        focus: 'series'
                    },
                    data: valus,
                    //itemStyle: {
                    //    color: 'rgba(238, 232, 170, 1)',
                    //},
                    label: {
                        show: true,
                        //rotate: 25,
                        //position: 'center',
                        //align: 'left',
                        fontSize: 14,
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
                        //formatter: function (params) {

                        //    return parseFloat(params.value).toFixed(0);

                        //}
                    }

                }

            ]
        };
        option && myChart.setOption(option);
        myChart.resize();


    }
})
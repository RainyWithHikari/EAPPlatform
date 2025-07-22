layui.use(['table', 'jquery', 'laydate'], function () {
    var table = layui.table
        , $ = layui.jquery
        , form = layui.form
        , laydate = layui.laydate;
    form.render();
    form.render("select");
    const today = new Date();
    const datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
    $("#alarmDateStart").val(datetime);
    $("#alarmDateEnd").val(datetime);


    laydate.render({
        elem: '#datepicker'

        //, value: date
        , done: function (value, date, endDate) {
            //console.log(value)
            getData($("#equipment").val(), $("#datepicker").val());
        }

    });

    laydate.render({
        elem: '#alarmDateStart'

        //, range:true

        , done: function (value) {
            console.log($("#alarmDateStart").val())

            if ($("#alarmDateEnd").val() != null) {
                if ($("#alarmDateStart").val() <= $("#alarmDateEnd").val()) {
                    GetAlarmDetails(eqp, $("#alarmDateStart").val(), $("#alarmDateEnd").val())
                } else {
                    layer.msg("结束日期不可小于开始日期！");
                }
            } else {
                layer.msg("结束日期不可为空！");
            }
            //console.log(date)
            //console.log(endDate)
            //getData($("#equipment").val(), $("#datepicker").val());
        }

    });
    laydate.render({
        elem: '#alarmDateEnd'

        //, range:true

        , done: function (value) {
            //console.log(value)
            if ($("#alarmDateStart").val() != null) {
                if ($("#alarmDateStart").val() <= $("#alarmDateEnd").val()) {
                    GetAlarmDetails(eqp, $("#alarmDateStart").val(), $("#alarmDateEnd").val())
                } else {
                    layer.msg("结束日期不可小于开始日期！");
                }
            } else {
                layer.msg("开始日期不可为空！");
            }
            //
            
            //console.log(date)
            //console.log(endDate)
            //getData($("#equipment").val(), $("#datepicker").val());
        }

    });


    //var datetime = "@DateTime.Today.ToString("yyyy-MM - dd")";
    $("#equipment").val(eqp);
    $("#datepicker").val(datetime);
    $("#exportStatusDetails").click(function () {
        var now = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
        var eqp = $("#equipment").val();
        var datetime = $("#datepicker").val();
        if (datetime == now) {
            layer.msg('正在下载！数据范围: 当前24小时');
        } else {
            layer.msg(`正在下载！数据范围: ${datetime} 9:00 am 至 次日 9:00 am`);
        }

        ExportStatusData(eqp, datetime);


    })

    getData(eqp, datetime);

    function GetAlarmDetails(eqp, starttime, endtime) {
        console.log(eqp)
        console.log(starttime)
        console.log(endtime)
        // check if the endtime is later than starttime]
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": eqp,//$("#equipment").val(),
                "startdate": starttime,// $("#datepicker").val(),
                "enddate": endtime
            },
            url: '/Statistics/EquipmentDashboard/GetAlarmDetails',
            success: function (retdata) {

                table.render({
                    elem: '#alarmhist'
                    , id: "alarmTable"
                    , toolbar
                    , cols: [[ //标题栏
                        { field: 'ALARMTIME', title: 'Alarm Time' }
                        , { field: 'ALARMCODE', title: 'Alarm Code' }
                        , { field: 'ALARMTEXT', title: 'Alarm Text' }


                    ]]
                    , data: retdata.data
                    , size: 'sm'
                    , even: true
                    , limit: retdata.data.length
                });

            },
            error: function () {

            }
        });

    }
    function getData(eqp, datetime) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "equipment": eqp,//$("#equipment").val(),
                "datetime": datetime,// $("#datepicker").val(),
            },
            url: '/Statistics/EquipmentDashboard/GetDetailsData',
            success: function (retdata) {

                var alarmdata = retdata.alarmdata;

                var trenddates = retdata.trenddata.dates;
                var trendmtbas = retdata.trenddata.mtbas;
                var trendrunrates = retdata.trenddata.runrates;
                var chartdata = retdata.chartdata;

                var status = retdata.status;
                var runrate = retdata.runrate;
                $("#status").val(status.STATUS);
                $("#runrate").val(runrate);

                $("#statusLastUpdate").html("")
                var lastUpdateTime = "最新Status更新时间：" + status.UPDATETIME
                $("#statusLastUpdate").html(lastUpdateTime)

                GetStatusDetails(chartdata)
                //10.26 Rainy Add (OEE Related)
                var parameterdata = retdata.parameterdata;
                //console.log(parameterdata);

                //gettrendchart('trendchart', trenddates, trendmtbas, trendrunrates);
                gettrendchart('trendrunrate', trenddates, trendrunrates, 'Run Rate');
                gettrendmtba('trendmtba', trenddates, trendmtbas, 'MTBA');

                getdatechart('datechart', chartdata);

                table.render({
                    elem: '#alarmhist'
                    , cols: [[ //标题栏
                        { field: 'ALARMTIME', title: 'Alarm Time' }
                        , { field: 'ALARMCODE', title: 'Alarm Code' }
                        , { field: 'ALARMTEXT', title: 'Alarm Text' }


                    ]]
                    , toolbar
                    , data: alarmdata
                    , size: 'sm'
                    , even: true
                    , limit: alarmdata.length
                });
                table.render({
                    elem: '#paramlist'
                    , cols: [[ //标题栏
                        { field: 'SVID', title: 'Param ID' }
                        , { field: 'Name', title: 'Param Name' }
                        , { field: 'Value', title: 'Param Value' }
                        , { field: 'UpdateTime', title: 'Update Time' }

                    ]]
                    , data: parameterdata
                    , size: 'sm'
                    , even: true
                    , limit: parameterdata.length
                });
            },
            error: function () {

            }
        });



    }
    function GetStatusDetails(chartdata) {
        $('#runtime').val("0")
        $('#idletime').val("0")
        $('#alarmtime').val("0");
        $('#downtime').val("0");
        // 使用 reduce() 方法将相同 name 的项的 duration 转换为数字并相加
        const result = chartdata.reduce((acc, curr) => {
            const durationArray = curr.duration.split(":").map(Number); // 将时间跨度字符串转换为数字数组
            const test = durationArray[2] / 60
            //console.log(durationArray[2] / 60)
            const durationInMinutes = (durationArray[0] * 60) + durationArray[1] + (durationArray[2]/60); // 将小时转换为分钟并相加
            acc[curr.name] = (acc[curr.name] || 0) + durationInMinutes; // 将相同 name 的项的 duration 相加
            return acc;
        }, {});

        // 输出结果
        for (const name in result) {
            console.log(`Name: ${name}, Total Duration in Minutes: ${result[name].toFixed(2) }`);
            

            if (name == 'Run') {
                $('#runtime').val(result[name].toFixed(2))
            }
            else if (name == 'Idle') $('#idletime').val(result[name].toFixed(2));
            else if (name == 'Alarm') $('#alarmtime').val(result[name].toFixed(2));
            else {
                $('#downtime').val(result[name].toFixed(2));
            }
        }

    }
    function ExportStatusData(eqp, datetime) {
        var params = {
            "EQID": eqp, "datetime": datetime
        }
        //console.log(eqp + ' ' + datetime)

        $.ajax({
            type: 'post',
            data: {
                "EQID": eqp,
                "datetime": datetime,
            },
            url: '/Statistics/EquipmentDashboard/ExportStatusData',
            success: function (response, status, request) {
                console.log('here')
                var disp = request.getResponseHeader('Content-Disposition');
                if (disp && disp.search('attachment' != -1)) {
                    var form = $('<form method="POST" action="' + '/Statistics/EquipmentDashboard/ExportStatusData' + '">');
                    $.each(params, function (k, v) {
                        form.append($('<input type="hidden" name="' + k +
                            '" value="' + v + '">'));
                    });
                    $('body').append(form);
                    form.submit();
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    }
})


function getdatechart(id, chartdata) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);

    $(window).on('resize', function () {
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = window.innerWidth * 0.98 + 'px';
        chartDom.style.height = window.innerHeight * 0.4 + 'px';
        myChart.resize();
    });
    chartDom.style.width = window.innerWidth * 0.98 + 'px';
    chartDom.style.height = window.innerHeight * 0.4 + 'px';

    function renderItem(params, api) {
        var categoryIndex = api.value(0);//value第一个参数
        //console.log(categoryIndex);
        var start = api.coord([api.value(1), categoryIndex]);
        var end = api.coord([api.value(2), categoryIndex]);
        //console.log(start);
        // console.log(end);
        var height = api.size([0, 1])[1] * 0.6;

        var rectShape = echarts.graphic.clipRectByRect({
            x: start[0],
            y: start[1] - height / 2,
            width: end[0] - start[0],
            height: height
        }, {
            x: params.coordSys.x,
            y: params.coordSys.y,
            width: params.coordSys.width,
            height: params.coordSys.height
        });

        return rectShape && {
            type: 'rect',
            transition: ['shape'],
            shape: rectShape,
            style: api.style()
        };
    }
    var option;
    option = {
        tooltip: {
            formatter: function (params) {
                if (params.data.alarmtext == "")
                    return params.marker + params.name + ': ' + params.value[1] + ' ~ ' + params.value[2] + ' Duration: ' + params.data.duration;
                else
                    return params.marker + params.name + ': ' + params.value[1] + ' ~ ' + params.value[2] + ' Duration: ' + params.data.duration + '<br>AlarmText:<br>' + params.data.alarmtext;
            }
        },
        //title: {
        //    text: 'Status Details'
        //},
        dataZoom: [{
            type: 'slider',
            filterMode: 'weakFilter',
            showDataShadow: false,
            //top: 400,
            labelFormatter: ''
        }, {
            type: 'inside',
            filterMode: 'weakFilter'
        }],
        grid: {
            left: '50px', // 设置左边内边距为 20 像素
            top: '20px'
            //height: 300
        },
        xAxis: {
            scale: true,
            type: 'time'

        },
        yAxis: {
            data: ['Status']
        },
        series: [{
            type: 'custom',
            renderItem: renderItem,
            // itemStyle: {
            //     opacity: 0.8
            // },
            encode: {
                x: [1, 2],
                y: 0
            },
            data: chartdata
        }]
    };

    option && myChart.setOption(option);
    myChart.resize();
}

function gettrendchart(id, trenddates, trendrunrates, chartname) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);

    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = window.innerWidth * 0.45 + 'px';
        chartDom.style.height = window.innerHeight * 0.6 + 'px';
        myChart.resize();
    });
    chartDom.style.width = window.innerWidth * 0.45 + 'px';
    chartDom.style.height = window.innerHeight * 0.6 + 'px';

    var option;

    option = {
        title: {
            text: chartname
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        legend: {
            //data: [chartname]
        },
        grid: {


        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: trenddates
        },
        yAxis: [
            {
                type: 'value',
                name: chartname,
                min: 0,
                max: 100,
                position: 'left',
                splitLine: {     //网格线
                    show: false
                },
                axisLabel: {
                    formatter: '{value} %'
                }
            }],
        series: [

            {
                name: chartname,
                type: 'line',
                smooth: true,
                //yAxisIndex: 1,
                data: trendrunrates,
                label: {
                    show: true,
                    position: 'top',
                    formatter: '{c} %'
                },
                lineStyle: {
                    color: 'blue'
                }

            }
        ]
    };
    option && myChart.setOption(option);
    myChart.resize();


}
function gettrendmtba(id, trenddates, trendrunrates, chartname) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);

    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = window.innerWidth * 0.45 + 'px';
        chartDom.style.height = window.innerHeight * 0.6 + 'px';
        myChart.resize();
    });
    chartDom.style.width = window.innerWidth * 0.45 + 'px';
    chartDom.style.height = window.innerHeight * 0.6 + 'px';

    var option;

    option = {
        title: {
            text: chartname
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        legend: {
            //data: [chartname]
        },
        grid: {

        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: trenddates
        },
        yAxis: [
            {
                type: 'value',
                name: chartname,
                min: 0,
                //max: 100,
                position: 'left',
                splitLine: {     //网格线
                    show: false
                },
                axisLabel: {
                    //formatter: '{value} %'
                }
            }],
        series: [

            {
                name: chartname,
                type: 'line',
                smooth: true,
                //yAxisIndex: 1,
                data: trendrunrates,
                label: {
                    show: true,
                    position: 'top',
                    //formatter: '{c} %'
                },
                lineStyle: {
                    color: 'blue'
                }

            }
        ]
    };
    option && myChart.setOption(option);
    myChart.resize();


}
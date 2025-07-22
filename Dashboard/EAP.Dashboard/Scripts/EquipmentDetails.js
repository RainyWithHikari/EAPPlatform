layui.use(['table', 'jquery', 'laydate', 'element'], function () {
    var table = layui.table
        , $ = layui.jquery
        , form = layui.form
        , laydate = layui.laydate
        , element = layui.element;
    form.render();
    form.render("select");
    var alarmtable;
    const today = new Date();
    const datetime = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
    

    const params = new URLSearchParams(window.location.search);
    const eqp = params.get("eqp");  // 自动解码为 ABC#123

    $("#alarmDateStart").val(datetime);
    $("#alarmDateEnd").val(datetime);
    $("#equipment").html(eqp);
    $("#datepicker").html(datetime);

    laydate.render({
        elem: '#datepicker',
        lang:'en'
        , done: function (value, date, endDate) {
            //console.log(value)
            getData($("#equipment").html(), $("#datepicker").html());
        }

    });
    // 开始日期

    laydate.render({
        elem: '#btn-start-date',
        trigger: 'click',
        done: function (value) {
            const text = document.getElementById('start-date-text');
            if (text) text.innerText = value || '选择开始日期';
            document.getElementById('start-date-hidden').value = value;
        }
    });
    // 结束日期
    laydate.render({
        elem: '#btn-end-date',
        trigger: 'click',
        done: function (value) {
            const text = document.getElementById('end-date-text');
            if (text) text.innerText = value || '选择结束日期';
            document.getElementById('start-date-hidden').value = value;
        }
    });
    //执行一个laydate实例
    laydate.render({
        elem: '#startdateFilter',
        done: function () {
            $('.layui-form').css('overflow', 'auto'); // 恢复
        },
        ready: function () {
            $('.layui-form').css('overflow', 'visible'); // 打开前改成 visible
        }//指定元素

    });
    //执行一个laydate实例
    laydate.render({
        elem: '#enddateFilter' //指定元素

    });

    element.on('tab(deviceTab)', function (data) {
        if (data.index === 1) {
            echarts.getInstanceByDom(document.getElementById('trendrunrate'))?.resize();
            echarts.getInstanceByDom(document.getElementById('trendmtba'))?.resize();
        }
    });
    //监听提交
    form.on('submit(formDownload)', function (data) {
        console.log(data.field)
        //layer.msg(JSON.stringify(data.field));
        var originalObject = data.field
        var eqps = eqp;

        var startdate = data.field.startdate;
        var date = data.field.date;
        if (startdate > date) {
            layer.alert("开始时间不可小于结束时间！");
            return false;
        }
        //var duration = data.field.Duration;
        var reports = Object.keys(originalObject) // 获取对象的所有属性名
            .filter(key => key.startsWith('report')) // 筛选出以EQID开头的属性名
            .map(key => originalObject[key]); // 获取这些属性对应的值并存入新数组
        //console.log(date)
        //console.log(duration)
        //console.log(reports)

        if (eqps.length == 0) {
            layer.msg("请选择设备！")
            return false;
        }
        layer.msg("报表下载中，请稍后……");
        // 禁用确认按钮
        var $confirmBtn = $('#formDownload');
        $confirmBtn.addClass('layui-btn-disabled').attr('disabled', true).text('数据计算中...');
        DownloadData(eqps, date, startdate, reports)

        return true;
    });


    function DownloadData(eqps, date, startdate, reports) {
        const xhr = new XMLHttpRequest();
        xhr.open("POST", "DownloadReportData", true);
        xhr.responseType = "blob"; // 👈 blob 类型，不能访问 responseText！

        xhr.onload = function () {
            if (xhr.status === 200) {
                const blob = xhr.response;
                const link = document.createElement('a');
                const filename = `${eqps}_${startdate}_${date}.zip`;

                link.href = window.URL.createObjectURL(blob);
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);

                layer.msg('<em style="color:white;">下载成功！</em>');
            } else {
                layer.msg('<em style="color:white;">下载失败！</em>');
                console.error("下载失败，状态码：" + xhr.status);
            }

            $('#formDownload').removeClass('layui-btn-disabled').removeAttr('disabled').text('下载报表');
        };

        xhr.onerror = function () {
            layer.msg('<em style="color:white;">下载失败！</em>');
            $('#formDownload').removeClass('layui-btn-disabled').removeAttr('disabled').text('下载报表');
        };

        const formData = new FormData();
        formData.append("eqps", eqps);
        formData.append("datetime", date);
        formData.append("starttime", startdate);
        // ✅ 重点：把数组里的每个值都 append 成单个 reports[]
        reports.forEach(report => {
            formData.append("reports", report); // 或者使用 reports[]，看你后端是否需要
        });

        xhr.send(formData);
    }

    laydate.render({
        elem: '#alarmDateStart',
        fixed: false,
        lang: 'en'
        , done: function (value) {
            console.log($("#alarmDateStart").val())

            if ($("#alarmDateEnd").val() != null) {
                if ($("#alarmDateStart").val() <= $("#alarmDateEnd").val()) {
                    GetAlarmDetails(eqp)
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
        elem: '#alarmDateEnd',
        fixed: false,
        lang: 'en'
        //, range:true

        , done: function (value) {
            //console.log(value)
            if ($("#alarmDateStart").val() != null) {
                if ($("#alarmDateStart").val() <= $("#alarmDateEnd").val()) {
                    GetAlarmDetails(eqp)
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

    $("#exportStatusDetails").click(function () {
        //var now = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
        //var eqp = $("#equipment").val();
        //var datetime = $("#datepicker").val();
        //if (datetime == now) {
        //    layer.msg('正在下载！数据范围: 当前24小时');
        //} else {
        //    layer.msg(`正在下载！数据范围: ${datetime} 9:00 am 至 次日 9:00 am`);
        //}

        //ExportStatusData(eqp, datetime);


    })
    $("#exportAlarmDetails").click(function () {
        console.log('test')
        var eqp = $("#equipment").val();
        var starttime = $("#alarmDateStart").val();
        var endtime = $("#alarmDateEnd").val();

        layer.msg(`正在下载！数据范围: ${starttime} 至 ${endtime}`);

        ExportAlarmData(eqp, starttime, endtime)


    })
    getData(eqp, datetime);

    function GetAlarmDetails(eqp) {
        var starttime = $("#alarmDateStart").val();
        var endtime = $("#alarmDateEnd").val();

        alarmtable = table.render({
            elem: '#alarmhist'
            , id: "alarmhist"
            , url: 'GetEqpAlarmDetails'
            , title: 'Alarm List'
            , height: 'auto'
            , cols: [[ //标题栏
                { field: 'START', title: 'Start Time', templet: '<div>{{ FormDate(d.START, "yyyy-MM-dd HH:mm:ss") }}</div>', align: 'center' }
                , { field: 'END', title: 'End Time', templet: '<div>{{ FormDate(d.END, "yyyy-MM-dd HH:mm:ss") }}</div>', align: 'center' }
                , { field: 'ALARMCODE', title: 'Alarm Code', align: 'center' }
                , { field: 'ALARMTEXT', title: 'Alarm Text', align: 'center' }
                , { field: 'Duration', title: 'Duration(min)', align: 'center' }

            ]]
            , where: {//这里传参 向后台
                "EQID": eqp,//$("#equipment").val(),
                "startdate": starttime,// $("#datepicker").val(),
                "enddate": endtime

            }
            , toolbar: "#toolbar2"
            , page: true //开启分页
            , limits: [8, 16, 32]

            //, data: alarmdata

            , even: true
            , limit: 8

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
            url: 'GetDetailsData',
            success: function (retdata) {



                var trenddates = retdata.trenddata.dates;
                var trendmtbas = retdata.trenddata.mtbas;
                var trendrunrates = retdata.trenddata.runrates;
                var chartdata = retdata.chartdata;

                var status = retdata.status;
                console.log(status);
                var runrate = retdata.runrate;
                /*$("#status").html(status.Status);*/
                updateStatusBadge(status.Status)
                $("#runrate").html(runrate + '%');

                //$("#statusLastUpdate").html("")
                //var lastUpdateTime = "最新Status更新时间：" + status.UpdateTime
                //$("#statusLastUpdate").html(lastUpdateTime)

                GetStatusDetails(chartdata)
                //10.26 Rainy Add (OEE Related)
                var parameterdata = retdata.parameterdata;
                //console.log(parameterdata);

                //gettrendchart('trendchart', trenddates, trendmtbas, trendrunrates);
                gettrendchart('trendrunrate', trenddates, trendrunrates, 'Run Rate');
                gettrendmtba('trendmtba', trenddates, trendmtbas, 'MTBA');

                getdatechart('datechart', chartdata);

                table.render({
                    elem: '#paramlist'
                    , cols: [[ //标题栏
                        { field: 'SVID', title: 'Param ID', align: 'center' }
                        , { field: 'Name', title: 'Param Name', align: 'center' }
                        , { field: 'Value', title: 'Param Value', align: 'center' }
                        , { field: 'UpdateTime', title: 'Update Time', templet: '<div>{{ FormDate(d.UpdateTime, "yyyy-MM-dd HH:mm:ss") }}</div>', align: 'center' }


                    ]]
                    , data: parameterdata

                    , even: true
                    , limit: parameterdata.length
                });
            },
            error: function () {

            }
        });


        GetAlarmDetails(eqp)
    }

    function updateStatusBadge(status) {
        var statusText = status || "UNKNOWN";

        // 移除原有状态类
        $("#status").removeClass("run idle alarm down offline unknown").addClass(statusText.toLowerCase());

        // 更新文本
        $("#status").text(statusText);
    }
    function GetStatusDetails(chartdata) {


        // 使用 reduce() 方法将相同 name 的项的 duration 转换为数字并相加
        const result = chartdata.reduce((acc, curr) => {
            
            var durationInMinutes;


            durationInMinutes = curr.duration.TotalMinutes || 0;// (durationArray[0] * 60) + durationArray[1] + (durationArray[2] / 60); // 将小时转换为分钟并相加

            acc[curr.name] = (acc[curr.name] || 0) + durationInMinutes; // 将相同 name 的项的 duration 相加
            return acc;
        }, {});






        if (!result.hasOwnProperty('Run')) {
            result.Run = 0;
        }

        if (!result.hasOwnProperty('Idle')) {
            result.Idle = 0;
        }

        if (!result.hasOwnProperty('Alarm')) {
            result.Alarm = 0;
        }
        if (!result.hasOwnProperty('Down')) {
            result.Down = 0;
        }
        if (result.hasOwnProperty('Offline')) {

            result.Offline = result['Offline'];

        }


        var downrate = (result.Alarm / (result.Run + result.Idle + result.Alarm))*100

        $("#downrate").html(downrate.toFixed(2)+'%');
        var tableData = [];
        tableData.push(result);
        const colorMap = {
            Run: '#28a745',    // green
            Idle: '#ffc107',   // yellow
            Alarm: '#dc3545',  // red
            Down: '#6c757d',    // gray
            Offline: '#343a40'    // gray
        };

        const displayNames = {
            Run: 'Run',
            Idle: 'Idle',
            Alarm: 'Alarm',
            Down: 'Down',
            Offline:'Offline'
        };

        let html = '<div class="status-block-wrap">';
        Object.entries(result).forEach(([key, val]) => {
            const color = colorMap[key] || '#ccc';
            const label = displayNames[key] || key;
            html += `
        <div class="status-block">
            <span class="status-dot" style="background-color: ${color};"></span>
            <span class="status-name">${label}:</span>
            <span class="status-value">${val.toFixed(1)} min</span>
        </div>
    `;
        });
        html += '</div>';

        $('#status-summary').html(html);
        // 输出结果

        /* console.log(tableData)*/

        //table.render({
        //    elem: '#statuslist'
        //    , height: 'auto'
        //    , id: "statuslist"
        //    , toolbar: "#toolbar1"
        //    //, width: '100%'
        //    , cols: [[ //标题栏
        //        { field: 'Run', title: 'Run Time (min)', width: '25%' }
        //        , { field: 'Idle', title: 'Idle Time (min)', width: '25%' }
        //        , { field: 'Alarm', title: 'Alarm Time (min)', width: '25%' }
        //        , { field: 'Down', title: 'Down Time (min)', width: '25%' }

        //    ]]
        //    , data: tableData

        //    , even: true
        //    , limit: tableData.length
        //});

    }
    table.on('toolbar(alarmhist)', function (obj) {
        console.log('test')
        var eqp = $("#equipment").val();
        var starttime = $("#alarmDateStart").val();
        var endtime = $("#alarmDateEnd").val();

        layer.msg(`正在下载！数据范围: ${starttime} 至 ${endtime}`);

        ExportAlarmData(eqp, starttime, endtime)
    })
    table.on('toolbar(statuslist)', function (obj) {
        var now = today.getFullYear() + '-' + (today.getMonth() + 1).toString().padStart(2, '0') + '-' + today.getDate().toString().padStart(2, '0');
        var eqp = $("#equipment").val();
        var datetime = $("#datepicker").val();
        if (datetime == now) {
            layer.msg('正在下载！数据范围: 当前24小时');
        } else {
            layer.msg(`正在下载！数据范围: ${datetime} 9:00 am 至 次日 9:00 am`);
        }
        ExportStatusData(eqp, datetime)
    });
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
            url: 'ExportStatusData',
            success: function (response, status, request) {
                console.log('here')
                var disp = request.getResponseHeader('Content-Disposition');
                if (disp && disp.search('attachment' != -1)) {
                    var form = $('<form method="POST" action="' + 'ExportStatusData' + '">');
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

    function ExportAlarmData(eqp, starttime, endtime) {
        var params = {
            "EQID": eqp, "starttime": starttime, "endtime": endtime,
        }
        //console.log(eqp + ' ' + datetime)

        $.ajax({
            type: 'post',
            data: {
                "EQID": eqp,
                "starttime": starttime,
                "endtime": endtime,
            },
            url: 'ExportAlarmData',
            success: function (response, status, request) {
                console.log('here')
                var disp = request.getResponseHeader('Content-Disposition');
                if (disp && disp.search('attachment' != -1)) {
                    var form = $('<form method="POST" action="' + 'ExportAlarmData' + '">');
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

    function getdatechart(id, chartdata) {
        var chartDom = document.getElementById(id);
        var myChart = echarts.init(chartDom);
        $(window).on('resize', function () {//
            //屏幕大小自适应，重置容器高宽
            chartDom.style.width = chartDom.getBoundingClientRect().width;
            chartDom.style.height = chartDom.getBoundingClientRect().height;
            myChart.resize();
        });
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        //$(window).on('resize', function () {
        //    //屏幕大小自适应，重置容器高宽
        //    chartDom.style.width = window.innerWidth  + 'px';
        //    chartDom.style.height = window.innerHeight * 0.4 + 'px';
        //    myChart.resize();
        //});
        //chartDom.style.width = window.innerWidth + 'px';
        //chartDom.style.height = window.innerHeight * 0.4 + 'px';

        function renderItem(params, api) {
            var categoryIndex = api.value(0);//value第一个参数
            //console.log(categoryIndex);
            var start = api.coord([api.value(1), categoryIndex]);
            var end = api.coord([api.value(2), categoryIndex]);
            //console.log(start);
            // console.log(end);
            var height = api.size([0, 1])[1] * 0.3;

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
                        return params.marker + params.name + ': ' + params.value[1] + ' ~ ' + params.value[2] + ' Duration: ' + params.data.duration.TotalMinutes.toFixed(2) + ' min';
                    else
                        return params.marker + params.name + ': ' + params.value[1] + ' ~ ' + params.value[2] + ' Duration: ' + params.data.duration.TotalMinutes.toFixed(2) + ' min' + '<br>AlarmText:<br>' + params.data.alarmtext;
                }
            },
            //title: {
            //    text: 'Status Details'
            //},
            dataZoom: [
                {
                    type: 'slider',
                    filterMode: 'weakFilter',
                    showDataShadow: false,
                    labelFormatter: '',
                    top: '65%', // 将滑块拖动条放到顶部
                    height: 14
                },
                {
                    type: 'inside',
                    filterMode: 'weakFilter'
                }
            ],
            grid: {
                height:'60%',
                top: 20,
               /* bottom: 40,*/
                left: 40,
                right: 20,
                containLabel: true
            },
            xAxis: {
                scale: true,
                type: 'time',
                //axisLabel: {
                //    formatter: function (value) {
                //        // 使用 moment.js 或其他日期库来格式化日期和时间
                //        // console.log(value)
                //        const date = new Date(value);

                //        const formattedDate = date.toLocaleString();
                //        return formattedDate.;
                //    }
                //}

            },
            yAxis: {
                type: 'category',
                data: [''],
                axisLine: { show: true }, // 显示 x 轴线
                axisTick: { show: false },
                axisLabel: { show: false },
                splitLine: { show: false }
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
        const chartDom = document.getElementById(id);

        // 设置默认尺寸，防止为 0
        if (!chartDom.style.height || chartDom.offsetHeight === 0) {
            chartDom.style.height = '240px';
        }
        if (!chartDom.style.width || chartDom.offsetWidth === 0) {
            chartDom.style.width = '100%';
        }

        const myChart = echarts.init(chartDom);

        const option = {
            title: {
                text: chartname,
                left: 'left',
                top: 10
            },
            tooltip: {
                trigger: 'axis',
                axisPointer: { type: 'cross' }
            },
            grid: {
                left: '6%',
                right: '4%',
                top: 50,
                bottom: 40,
                containLabel: true
            },
            xAxis: {
                type: 'category',
                data: trenddates,
                axisTick: { alignWithLabel: true },
                axisLabel: {
                    rotate: 30,
                    interval: 0
                }
            },
            yAxis: {
                type: 'value',
                name: chartname,
                min: 0,
                max: 100,
                axisLabel: {
                    formatter: '{value} %'
                },
                splitLine: { show: false }
            },
            series: [{
                name: chartname,
                type: 'line',
                smooth: true,
                data: trendrunrates,
                label: {
                    show: true,
                    position: 'top',
                    formatter: '{c} %'
                },
                lineStyle: {
                    color: '#007bff'
                },
                itemStyle: {
                    color: '#007bff'
                }
            }]
        };

        myChart.setOption(option);

        // 延迟 resize 防止隐藏 tab 中初始化失败
        setTimeout(() => {
            myChart.resize();
        }, 100);
    }

    function gettrendmtba(id, trenddates, trendrunrates, chartname) {
        const chartDom = document.getElementById(id);

        // 设置默认尺寸，防止为 0
        if (!chartDom.style.height || chartDom.offsetHeight === 0) {
            chartDom.style.height = '240px';
        }
        if (!chartDom.style.width || chartDom.offsetWidth === 0) {
            chartDom.style.width = '100%';
        }

        const myChart = echarts.init(chartDom);

        const option = {
            title: {
                text: chartname,
                left: 'left',
                top: 10
            },
            tooltip: {
                trigger: 'axis',
                axisPointer: { type: 'cross' }
            },
            grid: {
                left: '6%',
                right: '4%',
                top: 50,
                bottom: 40,
                containLabel: true
            },
            xAxis: {
                type: 'category',
                data: trenddates,
                axisTick: { alignWithLabel: true },
                axisLabel: {
                    rotate: 30,
                    interval: 0
                }
            },
            yAxis: {
                type: 'value',
                name: chartname,
                min: 0,
                max: 1440,
                axisLabel: {
                    formatter: '{value}'
                },
                splitLine: { show: false }
            },
            series: [{
                name: chartname,
                type: 'line',
                smooth: true,
                data: trendrunrates,
                label: {
                    show: true,
                    position: 'top',
                    formatter: '{c}'
                },
                lineStyle: {
                    color: '#007bff'
                },
                itemStyle: {
                    color: '#007bff'
                }
            }]
        };

        myChart.setOption(option);

        // 延迟 resize 防止隐藏 tab 中初始化失败
        setTimeout(() => {
            myChart.resize();
        }, 100);

    }
})





function expenseInsight_2021(dataIN) {
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log(dataINJSON)
    var data_Insurance = []
    var data_PM = []
    var data_Utility = []
    var data_Taxes = []
    var data_Labor = []
    var data_Material = []
    var data_Misc = []
    var expensesForYear

    //dataINJSON.forEach(elem => { if (elem.Month == 'August') data_Aug.push({ label: elem.Category, y: elem.Amount })})

    dataINJSON.forEach(elem => {
        if (elem.Category == 'Insurance')
            data_Insurance.push({ label: elem.Month, y: elem.Amount })
        if (elem.Category == 'Property Manager')
            data_PM.push({ label: elem.Month, y: elem.Amount })
        if (elem.Category == 'Utilities')
            data_Utility.push({ label: elem.Month, y: elem.Amount })
        if (elem.Category == 'Taxes')
            data_Taxes.push({ label: elem.Month, y: elem.Amount })
        if (elem.Category == 'Labor')
            data_Labor.push({ label: elem.Month, y: elem.Amount })
        if (elem.Category == 'Material')
            data_Material.push({ label: elem.Month, y: elem.Amount })
        if (elem.Category == 'Misc')
            data_Misc.push({ label: elem.Month, y: elem.Amount })
        expensesForYear = elem.Year
    }
    )


    console.log("In 2021")
    


        var chart2021 = document.getElementById("expenseInsightChart_2021")
        console.log(chart2021)
        var chart2 = new CanvasJS.Chart("expenseInsightChart_2021", {
            theme: "light2",
            animationEnabled: true,
            title: {
                text: "Expenses for " + expensesForYear
            },
            axisY: {
                prefix: "$",
                margin: 20
            },
            legend: {
                cursor: "pointer",
                itemclick: toggleDataSeries
            },
            toolTip: {
                shared: true,
                content: toolTipFormatter
            },
            data: [{
                type: "column",
                showInLegend: true,
                name: "Insurance",
                color: "red",
                dataPoints: data_Insurance
            },
            {
                type: "column",
                showInLegend: true,
                name: "PM",
                color: "#0479cc",
                dataPoints: data_PM
            },
            {
                type: "column",
                showInLegend: true,
                name: "Utility",
                color: "#68d5c8",
                dataPoints: data_Utility
            },
            {
                type: "column",
                showInLegend: true,
                name: "Taxes",
                color: "#e6c54f",
                dataPoints: data_Taxes
            },
            {
                type: "column",
                showInLegend: true,
                name: "Labor",
                color: "#f9777f",
                dataPoints: data_Labor
            },
            {
                type: "column",
                showInLegend: true,
                name: "Material",
                color: "#4db9f2",
                dataPoints: data_Material
            },
            {
                type: "column",
                showInLegend: true,
                name: "Misc",
                color: "#ff6d41",
                dataPoints: data_Misc
            }

            ]
        });
        chart2.render();

        function toolTipFormatter(e) {
            var str = "";
            var total = 0;
            var str3;
            var str2;
            for (var i = 0; i < e.entries.length; i++) {
                var str1 = "<span style= \"color:" + e.entries[i].dataSeries.color + "\">" + e.entries[i].dataSeries.name + "</span>: <strong>" + e.entries[i].dataPoint.y + "</strong> <br/>";
                total = e.entries[i].dataPoint.y + total;
                str = str.concat(str1);
            }
            str2 = "<strong>" + e.entries[0].dataPoint.label + "</strong> <br/>";
            str3 = "<span style = \"color:Tomato\">Total: </span><strong>" + total + "</strong><br/>";
            return (str2.concat(str)).concat(str3);
        }

        function toggleDataSeries(e) {
            if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                e.dataSeries.visible = false;
            }
            else {
                e.dataSeries.visible = true;
            }
            chart.render();
        }


}
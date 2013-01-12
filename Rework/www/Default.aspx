<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=2.0, user-scalable=yes" />

    <title>ImageWall</title>

	<script src="scripts/jquery-1.8.3.js" type="text/javascript"></script>
	<script src="scripts/jquery-ui-1.9.2.custom.js" type="text/javascript"></script>
	<script src="scripts/modernizr.custom.js" type="text/javascript"></script>

	<script src="scripts/jquery.Spinner.js" type="text/javascript"></script>
	<link href="css/jquery.Spinner.css" type="text/css" rel="stylesheet" />

	<script src="scripts/jquery.HeaderBar.js" type="text/javascript"></script>
	<link href="css/jquery.HeaderBar.css" type="text/css" rel="stylesheet" />

	<link media="screen" href="css/PageNormal.css" type="text/css" rel="stylesheet" />

	<!-- Fonts -->
	<link href='http://fonts.googleapis.com/css?family=Clicker+Script|Raleway:100' rel='stylesheet' type='text/css'>
	<%--<link media="handheld, only screen and (max-width: 480px), only screen and (max-device-width: 480px)" href="mobile.css" type="text/css" rel="stylesheet" />--%>
</head>
<body>
	<div class="PageContainer">
		<!-- Header bar -->
		<div class="HeaderBar"></div>
		<script type="text/javascript">
			$(document).ready(function () {
				$(".HeaderBar").HeaderBar();
			});
		</script>

		<!-- Page Header -->
		<div class="PageHeader">
			<label>ImageWall</label>
		</div>

		<!-- Page Content -->
		<div class="PageContent">
			<script>
				$(document).ready(function () {
					$(".ContentLoader").Spinner();
				});
			</script>
			<div class="ContentLoader iw-Spinner" style="width: 60px; height: 60px;"></div>

			<div class="WallContainer">

			</div>
		</div>

		<!-- Page footer -->
		<div class="PageFooter">
			<%--<table>
				<tr>
					<td><a href="http://www.best.hr/code-challenge/v3.0" target="_blank"><img src="images/BESTCodeChallenge3Logo.png" alt="BEST Code Challenge 3.0" width="110" /></a></td>
					<td><a href="http://www.microsoft.com/sqlserver/en/us/default.aspx" target="_blank"><img src="images/MSSQL.png" alt="Microsoft SQL Server" width="90" /></a></td>
					<td>
						<div><a href="http://www.asp.net/web-api" target="_blank"><img src="images/dotNETWCF.png" alt=".NET Windows Communication Foundation" width="250" /></a></div>
						<div><a href="http://www.windowsphone.com/" target="_blank"><img src="images/WindowsPhone.png" alt="Windows Phone" width="175" /></a></div>
					</td>
				</tr>
			</table>--%>
			<div class="SponsorLinks">
				<a href="http://www.best.hr/code-challenge/v3.0" target="_blank"><img src="images/BESTCodeChallenge3Logo.png" alt="BEST Code Challenge 3.0" width="110" /></a>
				<a href="http://www.microsoft.com/sqlserver/en/us/default.aspx" target="_blank"><img src="images/MSSQL.png" alt="Microsoft SQL Server" width="90" /></a>
				<div>
					<div><a href="http://www.asp.net/web-api" target="_blank"><img src="images/dotNETWCF.png" alt=".NET Windows Communication Foundation" width="250" /></a></div>
					<div><a href="http://www.windowsphone.com/" target="_blank"><img src="images/WindowsPhone.png" alt="Windows Phone" width="175" /></a></div>
				</div>
			</div>
			<label>&#169; 2012 <a href="http://blog.toplek.net/" target="_blank">Aleksandar Toplek</a> | BEST Code Challenge 3.0</label>
		</div>
	</div>
</body>
</html>

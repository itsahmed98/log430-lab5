# Define services
$services = @(
    @{ name = "catalogue-service"; url = "http://catalogue-upstream"; path = "/catalogue"; strip_path = $true; upstream = $true; targets = @("catalogue-api-1:80", "catalogue-api-2:80") },
    @{ name = "vente-service"; url = "http://magasincentral-vente:80"; path = "/vente"; strip_path = $true },
    @{ name = "inventaire-service"; url = "http://magasincentral-inventaire:80"; path = "/inventaire"; strip_path = $true },
    @{ name = "administration-service"; url = "http://magasincentral-administration:80"; path = "/administration"; strip_path = $true },
    @{ name = "ecommerce-service"; url = "http://ecommerce-api:80"; path = "/ecommerce"; strip_path = $true; apiKeyRequired = $true }
)

# Clean, then create services, routes, upstreams
foreach ($s in $services) {
    # Clean up routes
    try {
        $routes = Invoke-RestMethod -Method GET -Uri "http://localhost:8001/services/$($s.name)/routes"
        foreach ($r in $routes.data) {
            Invoke-RestMethod -Method DELETE -Uri "http://localhost:8001/routes/$($r.id)"
        }
        Write-Host "Deleted routes for service: $($s.name)"
    }
    catch {}

    # Clean up plugins
    try {
        $plugins = Invoke-RestMethod -Method GET -Uri "http://localhost:8001/services/$($s.name)/plugins"
        foreach ($p in $plugins.data) {
            Invoke-RestMethod -Method DELETE -Uri "http://localhost:8001/plugins/$($p.id)"
        }
        Write-Host "Deleted plugins for service: $($s.name)"
    }
    catch {}

    # Clean up services
    try {
        Invoke-RestMethod -Method DELETE -Uri "http://localhost:8001/services/$($s.name)" -ErrorAction Stop
        Write-Host "Deleted existing service: $($s.name)"
    }
    catch {
        Write-Host "Service $($s.name) does not exist or could not be deleted."
    }

    # If upstream is specified, create it and targets
    if ($s.ContainsKey("upstream") -and $s.upstream) {
        try {
            Invoke-RestMethod -Method DELETE -Uri "http://localhost:8001/upstreams/catalogue-upstream" -ErrorAction Stop
            Write-Host "Deleted existing upstream: catalogue-upstream"
        }
        catch {}

        Invoke-RestMethod -Method POST -Uri "http://localhost:8001/upstreams" -Body @{ name = "catalogue-upstream" } -ContentType "application/x-www-form-urlencoded"
        Write-Host "Created upstream: catalogue-upstream"

        foreach ($target in $s.targets) {
            Invoke-RestMethod -Method POST -Uri "http://localhost:8001/upstreams/catalogue-upstream/targets" `
                -Body @{ target = $target } -ContentType "application/x-www-form-urlencoded"
            Write-Host "Added target $target to upstream"
        }
    }

    # Create the service
    $serviceBody = @{
        name = $s.name
        url  = $s.url
    } | ConvertTo-Json -Depth 10

    Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services" `
        -Body $serviceBody -ContentType "application/json"
    Write-Host "Created service: $($s.name)"

    # Create the route
    $routeBody = @{
        paths      = @($s.path)
        strip_path = $s.strip_path
    } | ConvertTo-Json -Depth 10

    Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services/$($s.name)/routes" `
        -Body $routeBody -ContentType "application/json"
    Write-Host "Created route for: $($s.path)"

    # Plugin file-log (logs dans /var/log/kong/access.log)
    $logPluginBody = @{
        name   = "file-log"
        config = @{
            path = "/var/log/kong/access.log"
        }
    } | ConvertTo-Json -Depth 10 -Compress

    Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services/$($s.name)/plugins" `
        -Body $logPluginBody -ContentType "application/json"

    Write-Host "Attached file-log plugin to service: $($s.name)"

    # # Attach key-auth plugin if required
    # if ($s.ContainsKey("apiKeyRequired") -and $s.apiKeyRequired) {
    #     Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services/$($s.name)/plugins" `
    #         -Body @{ name = "key-auth" } -ContentType "application/x-www-form-urlencoded"
    #     Write-Host "Attached key-auth plugin to service: $($s.name)"
    # }

    # Attach CORS plugin to every service
    $body = @{
        name   = "cors"
        config = @{
            origins         = @("http://localhost:8080", "https://localhost:7230")
            methods         = @("GET", "POST", "PUT", "DELETE", "OPTIONS")
            headers         = @("Accept", "Authorization", "Content-Type")
            exposed_headers = @("X-Custom-Header")
            credentials     = $true
            max_age         = 3600
        }
    } | ConvertTo-Json -Depth 5

    Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services/$($s.name)/plugins" `
        -Body $body -ContentType "application/json"

    Write-Host "Attached CORS plugin to service: $($s.name)"
}

# # Create consumer and API key for ecommerce
# try {
#     Invoke-RestMethod -Method POST -Uri "http://localhost:8001/consumers" `
#         -Body @{ username = "client-app" } -ContentType "application/x-www-form-urlencoded"
#     Write-Host "Created consumer: client-app"
# }
# catch {
#     Write-Host "Consumer already exists: client-app"
# }

# try {
#     $key = Invoke-RestMethod -Method POST -Uri "http://localhost:8001/consumers/client-app/key-auth" `
#         -ContentType "application/x-www-form-urlencoded"
#     Write-Host "Generated API key for client-app: $($key.key)"
# }
# catch {
#     Write-Host "Key already exists for client-app"
# }

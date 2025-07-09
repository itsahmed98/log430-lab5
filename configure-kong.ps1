$services = @(
    @{ name = "catalogue-service";      url = "http://magasincentral-catalogue:80";      path = "/catalogue";      strip_path = $true },
    @{ name = "vente-service";          url = "http://magasincentral-vente:80";          path = "/vente";          strip_path = $true },
    @{ name = "inventaire-service";     url = "http://magasincentral-inventaire:80";     path = "/inventaire";     strip_path = $true },
    @{ name = "administration-service"; url = "http://magasincentral-administration:80"; path = "/administration"; strip_path = $true },
    @{ name = "ecommerce-service";      url = "http://ecommerce-api:80";                 path = "/ecommerce";      strip_path = $true }
)

foreach ($s in $services) {
    # Supprimer le service s’il existe déjà
    try {
        Invoke-RestMethod -Method DELETE -Uri "http://localhost:8001/services/$($s.name)" -ErrorAction Stop
        Write-Host "Deleted existing service: $($s.name)"
    } catch {
        Write-Host "Service $($s.name) does not exist or could not be deleted."
    }

    # Créer le service
    Write-Host "Creating service: $($s.name)"
    try {
        $serviceBody = @{
            name = $s.name
            url  = $s.url
        } | ConvertTo-Json -Depth 10

        Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services" `
            -Body $serviceBody -ContentType "application/json"
    } catch {
        Write-Host "Failed to create service $($s.name)."
    }

    # Créer la route
    Write-Host "Creating route for path: $($s.path)"
    try {
        $routeBody = @{
            paths      = @($s.path)
            strip_path = $s.strip_path
        } | ConvertTo-Json -Depth 10

        Invoke-RestMethod -Method POST -Uri "http://localhost:8001/services/$($s.name)/routes" `
            -Body $routeBody -ContentType "application/json"
    } catch {
        Write-Host "Failed to create route $($s.path)."
    }
}

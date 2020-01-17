# EstouAVer
Trabalho de grupo desenvolvido para a Unidade Curricular Segurança de Informática

## Serviço (Daemon)

Os comandos de inicialização do serviço devem ser realizados pela linha de comandos (como administrador), o powershell não suporta o comando sc. 

### Instalar o serviço:
```
sc create EstouAVerService binpath= C:\path\completo\para\o\executal\EstouAVer.exe 
```

sc create EstouAVerService binpath= C:\Users\Frias\Desktop\EstouAVer_SI\EstouAVer\bin\Debug\netcoreapp3.0\EstouAVer.exe 

### Desinstalar o serviço

```Powershell
sc delete EstouAVerService 
```

### Começar o serviço
Existem dois modos de começar o serviço, com SHA256

```Powershell
sc start EstouAVerService sha256 username password directory databasePassword
```
sc start EstouAVerService sha256 bob alice C:\Users\Frias\Desktop\FicheirosSI bobob


ou com o HMAC:

```Powershell
sc start EstouAVerService hmac key username password directory databasePassword
```

### Parar o serviço

```Powershell
sc stop EstouAVerService
```
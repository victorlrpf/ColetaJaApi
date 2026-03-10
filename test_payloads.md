# Exemplos de Teste - ColetaJá API

Use estes JSONs para testar os endpoints no Swagger ou Postman.

## 1. Cadastro de Usuários (`POST /api/Auth/register`)

### Paciente (Type: 0)
```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "senhaSegura123",
  "type": 0
}
```

### Coletador (Type: 1)
```json
{
  "name": "Carlos Coletor",
  "email": "carlos@email.com",
  "password": "senhaSegura123",
  "type": 1
}
```

### Laboratório (Type: 2)
```json
{
  "name": "Laboratório Central",
  "email": "lab@email.com",
  "password": "senhaSegura123",
  "type": 2
}
```

---

## 2. Login (`POST /api/Auth/login`)
```json
{
  "email": "joao@email.com",
  "password": "senhaSegura123"
}
```

---

## 3. Fluxo do Paciente (Requer Token de Paciente)

### Adicionar Endereço (`POST /api/Patient/address`)
```json
{
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Apto 45",
  "neighborhood": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567"
}
```

### Solicitar Exame (`POST /api/Patient/request-exam`)
*Nota: Certifique-se de que o `examTypeId` e `addressId` existam no banco.*
```json
{
  "examTypeId": 1,
  "addressId": 1
}
```

---

## 4. Fluxo do Coletador (Requer Token de Coletador)

### Atualizar Status (`POST /api/Collector/update-status/{id}`)
*Envie o número do status no corpo (Ex: 2 para Coletado).*
```json
2
```

---

## 5. Fluxo do Laboratório (Requer Token de Laboratório)

### Inserir Resultado (`POST /api/Laboratory/insert-result/{id}`)
```json
"Resultado do exame: Negativo para substância X. Todos os níveis dentro da normalidade."
```

# Space Invaders

Um clone do clássico arcade **Space Invaders**, desenvolvido em **C# com WPF (.NET)**, com suporte a até **3 jogadores simultâneos** no mesmo teclado. O objetivo é destruir todas as ondas de invasores alienígenas antes que eles te alcancem — ou que suas munições acabem.

---

## 🎮 Conceito do Jogo

Inspirado no arcade original de 1978, este Space Invaders coloca um ou mais jogadores na parte inferior da tela, responsáveis por defender o planeta de hordas de invasores alienígenas que se movem horizontalmente pelo espaço. Os inimigos avançam da direita para a esquerda de forma contínua e disparam projéteis em direção aos jogadores e às caixas de munição. Conforme o número de inimigos diminui, os sobreviventes ficam mais rápidos e agressivos. A partida termina com a vitória caso todos os invasores sejam eliminados, ou com a derrota caso um jogador seja atingido por um projétil inimigo.

---

## 🕹️ Modos de Jogo

| Modo        | Descrição                                              |
|-------------|--------------------------------------------------------|
| SinglePlayer | 1 jogador enfrenta os invasores sozinho              |
| MultiPlayer  | 2 ou 3 jogadores compartilham a mesma tela e teclado |

Ao iniciar o jogo, uma caixa de diálogo pergunta quantos jogadores participarão (1 a 3).

---

## ⚙️ Funcionalidades

- **Ondas de inimigos** — 30 invasores são gerados em 3 linhas de altura diferentes no início de cada partida, com sprites variados.
- **Disparo inimigo inteligente** — cada invasor tenta atirar quando está posicionado acima de um jogador ou de uma caixa de munição.
- **Aceleração dos inimigos** — quando restam menos de 10 invasores em campo, a velocidade de movimento deles dobra, aumentando o desafio.
- **Sistema de munição** — cada jogador começa com **30 balas** e não pode atirar sem munição.
- **Caixas de munição (AmmoBox)** — surgem em posições aleatórias da tela em intervalos variáveis, reabastecendo o jogador com +30 balas ao serem coletadas. Podem ser destruídas pelos inimigos.
- **Balas coloridas por jogador** — cada jogador possui uma cor de projétil diferente para facilitar a identificação em partidas multiplayer.
- **Reinício rápido** — após o Game Over, pressione **Enter** para reiniciar imediatamente.
- **Contador de inimigos** — a interface exibe em tempo real quantos invasores ainda restam.

---

## 🎯 Controles Padrão

### Player 1 — 🔵 Balas Azuis

| Ação         | Tecla        |
|--------------|--------------|
| Mover Esquerda | `A`        |
| Mover Direita  | `D`        |
| Atirar         | `Ctrl Esq` |

### Player 2 — 🔴 Balas Vermelhas

| Ação         | Tecla        |
|--------------|--------------|
| Mover Esquerda | `,` (vírgula)  |
| Mover Direita  | `.` (ponto)    |
| Atirar         | `Espaço`       |

### Player 3 — 🟢 Balas Verdes

| Ação         | Tecla        |
|--------------|--------------|
| Mover Esquerda | `Num 4`    |
| Mover Direita  | `Num 6`    |
| Atirar         | `Ctrl Dir` |

### Geral

| Ação               | Tecla   |
|--------------------|---------|
| Reiniciar (Game Over) | `Enter` |

---

## 📐 Diagramas

### Diagrama de Casos De Uso (SinglePlayer)

<img width="909" height="676" alt="image" src="https://github.com/user-attachments/assets/7436b3d7-8efc-429b-9ed3-24702de935d2" />

### Diagrama de Casos De Uso (MultiPlayer)

<img width="764" height="1079" alt="image" src="https://github.com/user-attachments/assets/c2988203-ab05-4ff3-8414-de4ba1b74373" />

### Diagrama de Classe

<img width="1132" height="484" alt="image" src="https://github.com/user-attachments/assets/87e968c2-8442-48c4-88b7-f401783de038" />

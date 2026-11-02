#include <cmath>
#include <iostream>

int main() {
    double a, b, result = 0;
    int op;
    std::cout << "First number: ";
    if (!(std::cin >> a)) return 1;
    std::cout << "Operation (1=+, 2=-, 3=*, 4=/): ";
    if (!(std::cin >> op)) return 1;
    std::cout << "Second number: ";
    if (!(std::cin >> b)) return 1;
    switch (op) {
        case 1:
            result = a + b;
            break;
        case 2:
            std::cerr << "Operation is not implemented\n"; return 1;
            break;
        case 3:
            std::cerr << "Operation is not implemented\n"; return 1;
            break;
        case 4:
            std::cerr << "Operation is not implemented\n"; return 1;
            break;
        default:
            std::cerr << "Unknown operation\n";
            return 1;
    }
    if (!std::isfinite(result)) { std::cerr << "Invalid result\n"; return 1; }
    std::cout << "Result = " << result << '\n';
}

import {
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss'],
})
export class ChartComponent implements OnInit, OnChanges {
  @Input() statusUpdate!: Map<string, number>;
  public chart: any;
  totalCount: number = 0;
  @ViewChild('chartCanvas') chartCanvas!: ElementRef;
  chartInstance: Chart | null = null; // Store chart instance

  constructor() {}
  ngOnInit(): void {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['statusUpdate']) {
      this.createChart();
    }
  }
  createChart() {
    if (!this.statusUpdate) {
      console.error('statusUpdate is empty or undefined.');
      return;
    }
    // const canvas = this.chartCanvas.nativeElement as HTMLCanvasElement;

    // ✅ Destroy previous chart if it exists
    if (this.chart) {
      this.chart.destroy();
    }

    let labelsArray = [...this.statusUpdate.keys()]; // Convert iterator to an array
    let valuesArray = [...this.statusUpdate.values()]; // Convert iterator to an array
    let totalCount = 0;
    for (let item of valuesArray) totalCount += item;
    for (let i = 0; i < valuesArray.length; i++) {
      valuesArray[i] = (valuesArray[i] / totalCount) * 100;
    }
    Chart.register(ChartDataLabels);
    this.chart = new Chart('MyChart', {
      type: 'doughnut',
      data: {
        labels: labelsArray,
        datasets: [
          {
            label: 'Status Updates',
            data: valuesArray,
            backgroundColor: ['green', 'red', '#FFD700'],
            hoverOffset: 4,
          },
        ],
      },
      options: {
        aspectRatio: 2.5,
        maintainAspectRatio: false, // ✅ Allows custom sizing
        cutout: '60%' as any,
        plugins: {
          legend: {
            position: 'right', // ✅ Moves legend to the right
            align: 'start', // ✅ Moves legend slightly above
            labels: {
              usePointStyle: true,
              boxWidth: 10,
              padding: 20, // ✅ Increases spacing between legend texts
              font: { size: 12 },
            },
          },
          datalabels: {
            // ✅ Data Labels Configuration
            color: '#fff', // Text color
            anchor: 'center', // Positioning
            align: 'center', // Center-align text
            font: {
              size: 9, // Adjust text size
              weight: 'bold',
            },
            formatter: (value) => value.toFixed(2) + '%', // Shows actual values inside segments
          },
        },
        layout: {
          padding: { left: 0, right: 20, top: 10, bottom: 10 }, // ✅ Adjusts overall chart padding
        },
        // } as ChartOptions<'doughnut'>,
      },
      plugins: [],
    });
  }
}
